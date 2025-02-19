using DCL.Helpers;
using Decentraland.Sdk.Ecs6;
using MainScripts.DCL.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AvatarModel : BaseModel
{
    [Serializable]
    public class AvatarEmoteEntry
    {
        public int slot;
        public string urn;
    }

    public string id;
    public string name;
    public string bodyShape;

    public Color skinColor;
    public Color hairColor;
    public Color eyeColor;

    public List<string> wearables = new();
    public HashSet<string> forceRender = new();
    public List<AvatarEmoteEntry> emotes = new();

    public string expressionTriggerId = null;
    public long expressionTriggerTimestamp = -1;
    public bool talking = false;

    public static AvatarModel FallbackModel(string name, int id) =>
        new()
        {
            id = $"{name}_{id}",
            name = name,
            bodyShape = "urn:decentraland:off-chain:base-avatars:BaseMale",

            skinColor = new Color(0.800f, 0.608f, 0.467f),
            hairColor = new Color(0.596f, 0.373f, 0.216f),
            eyeColor = new Color(0.373f, 0.224f, 0.196f),
        };

    public override BaseModel GetDataFromPb(ComponentBodyPayload pbModel)
    {
        if (pbModel.PayloadCase != ComponentBodyPayload.PayloadOneofCase.AvatarShape)
            return Utils.SafeUnimplemented<AvatarModel, AvatarModel>(expected: ComponentBodyPayload.PayloadOneofCase.AvatarShape, actual: pbModel.PayloadCase);

        var model = new AvatarModel();
        if (pbModel.AvatarShape.HasId) model.id = pbModel.AvatarShape.Id;
        if (pbModel.AvatarShape.HasName) model.name = pbModel.AvatarShape.Name;
        if (pbModel.AvatarShape.HasTalking) model.talking = pbModel.AvatarShape.Talking;
        if (pbModel.AvatarShape.HasBodyShape) model.bodyShape = pbModel.AvatarShape.BodyShape;
        if (pbModel.AvatarShape.EyeColor != null) model.eyeColor = pbModel.AvatarShape.EyeColor.AsUnityColor();
        if (pbModel.AvatarShape.HairColor != null) model.hairColor = pbModel.AvatarShape.HairColor.AsUnityColor();
        if (pbModel.AvatarShape.SkinColor != null) model.skinColor = pbModel.AvatarShape.SkinColor.AsUnityColor();
        if (pbModel.AvatarShape.HasExpressionTriggerId) model.expressionTriggerId = pbModel.AvatarShape.ExpressionTriggerId;
        if (pbModel.AvatarShape.HasExpressionTriggerTimestamp) model.expressionTriggerTimestamp = pbModel.AvatarShape.ExpressionTriggerTimestamp;
        if (pbModel.AvatarShape.Wearables is { Count: > 0 })
        {
            Debug.Log("进入这pbModel.AvatarShape.Wearables is { Count: > 0 }");
            
            model.wearables = pbModel.AvatarShape.Wearables.ToList();
            foreach (string collectionId in model.wearables)
            {
                //Debug.Log($"很好 collectionId: {collectionId}");
            }
        }

        if (pbModel.AvatarShape.Emotes is { Count: > 0 })
        {
            model.emotes = new List<AvatarEmoteEntry>(pbModel.AvatarShape.Emotes.Count);

            for (var i = 0; i < pbModel.AvatarShape.Emotes.Count; i++)
            {
                if (pbModel.AvatarShape.Emotes[i] == null) continue;
                AvatarEmoteEntry emote = new AvatarEmoteEntry();

                if (pbModel.AvatarShape.Emotes[i].HasSlot) emote.slot = pbModel.AvatarShape.Emotes[i].Slot;
                if (pbModel.AvatarShape.Emotes[i].HasUrn) emote.urn = pbModel.AvatarShape.Emotes[i].Urn;
                model.emotes.Add(emote);
            }
        }

        return model;
    }

    private bool IsListContainedIn(List<string> first, List<string> second)
    {
        foreach (string item in first)

            // This contains is slow
            if (!second.Contains(item))
                return false;

        return true;
    }

    private bool IsHashSetContainedIn(HashSet<string> first, HashSet<string> second)
    {
        foreach (string item in first)
            if (!second.Contains(item))
                return false;

        return true;
    }

    public bool HaveSameWearablesAndColors(AvatarModel other)
    {
        if (other == null)
            return false;

        //wearables are the same
        if (!(wearables.Count == other.wearables.Count
              && IsListContainedIn(wearables, other.wearables)
              && IsListContainedIn(other.wearables, wearables)))
            return false;

        if (!(forceRender.Count == other.forceRender.Count
              && IsHashSetContainedIn(forceRender, other.forceRender)
              && IsHashSetContainedIn(other.forceRender, forceRender)))
            return false;

        //emotes are the same
        if (emotes == null && other.emotes != null)
            return false;

        if (emotes != null && other.emotes == null)
            return false;

        if (emotes != null && other.emotes != null)
        {
            if (emotes.Count != other.emotes.Count)
                return false;

            foreach (AvatarEmoteEntry emote in emotes)
            {
                var found = false;

                foreach (AvatarEmoteEntry t in other.emotes)
                    if (t.urn == emote.urn)
                    {
                        found = true;
                        break;
                    }

                if (!found)
                    return false;
            }
        }

        return bodyShape == other.bodyShape &&
               skinColor == other.skinColor &&
               hairColor == other.hairColor &&
               eyeColor == other.eyeColor;
    }

    public bool Equals(AvatarModel other)
    {
        if (other == null) return false;

        bool wearablesAreEqual = wearables.All(other.wearables.Contains)
                                 && other.wearables.All(wearables.Contains)
                                 && wearables.Count == other.wearables.Count;

        bool forceRenderAreEqual = forceRender.All(other.forceRender.Contains)
                                   && other.forceRender.All(forceRender.Contains)
                                   && forceRender.Count == other.forceRender.Count;

        return id == other.id
               && name == other.name
               && bodyShape == other.bodyShape
               && skinColor == other.skinColor
               && hairColor == other.hairColor
               && eyeColor == other.eyeColor
               && expressionTriggerId == other.expressionTriggerId
               && expressionTriggerTimestamp == other.expressionTriggerTimestamp
               && wearablesAreEqual
               && forceRenderAreEqual;
    }

    public void CopyFrom(AvatarModel other)
    {
        Debug.Log("copyfrom的debug");
        Debug.Log(string.Join(",", wearables));

        if (other == null)
            return;

        id = other.id;
        name = other.name;
        bodyShape = other.bodyShape;
        skinColor = other.skinColor;
        hairColor = other.hairColor;
        eyeColor = other.eyeColor;
        expressionTriggerId = other.expressionTriggerId;
        expressionTriggerTimestamp = other.expressionTriggerTimestamp;
        wearables = new List<string>(other.wearables);
        emotes = other.emotes.Select(x => new AvatarEmoteEntry() { slot = x.slot, urn = x.urn }).ToList();
        forceRender = new HashSet<string>(other.forceRender);
    }

    public override BaseModel GetDataFromJSON(string json) =>
        Utils.SafeFromJson<AvatarModel>(json);
}
