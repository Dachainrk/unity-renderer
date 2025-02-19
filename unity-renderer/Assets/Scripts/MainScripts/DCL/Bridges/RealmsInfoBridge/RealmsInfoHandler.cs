using Cysharp.Threading.Tasks;
using DCL.Interface;
using UnityEngine;
using System;
using Variables.RealmsInfo;
using System.Collections.Generic;
using System.Linq;
using Decentraland.Bff;
using Google.Protobuf;
using System.Threading;

namespace DCL
{
    public class RealmsInfoHandler : IRealmsInfoBridge
    {
        private RealmsInfoModel model = new RealmsInfoModel();

        public CurrentRealmVariable playerRealm => DataStore.i.realm.playerRealm;
        public BaseCollection<RealmModel> realmsInfo => DataStore.i.realm.realmsInfo;
        private BaseVariable<AboutResponse.Types.AboutConfiguration> playerRealmAboutConfiguration => DataStore.i.realm.playerRealmAboutConfiguration;
        private BaseVariable<AboutResponse.Types.LambdasInfo> playerRealmAboutLambda => DataStore.i.realm.playerRealmAboutLambdas;
        private BaseVariable<AboutResponse.Types.ContentInfo> playerRealmAboutContent => DataStore.i.realm.playerRealmAboutContent;

        private BaseVariable<string> realmName => DataStore.i.realm.realmName;
        private UniTaskCompletionSource<IReadOnlyList<RealmModel>> fetchRealmsTask;

        public void Set(string json)
        {
            Debug.Log("Set里的json = " + json);
            JsonUtility.FromJsonOverwrite(json, model);
            Set(model);
        }

        internal void Set(RealmsInfoModel newModel)
        {
            model = newModel;

            if (!string.IsNullOrEmpty(model.current?.serverName))
            {
                Debug.Log("进入这");
               
                DataStore.i.realm.playerRealm.Set(model.current.Clone());
                realmName.Set(DataStore.i.realm.playerRealm.Get().serverName);
                Debug.Log("realmname = " + realmName.Get());
                Debug.Log("contentServerUrl = " + DataStore.i.realm.playerRealm.Get().contentServerUrl);
                Debug.Log("domain = " + DataStore.i.realm.playerRealm.Get().domain);
                Debug.Log("layer = " + DataStore.i.realm.playerRealm.Get().layer);
            }

            List<RealmModel> realms = newModel.realms != null ? newModel.realms.ToList() : new List<RealmModel>();
            Debug.Log("newModel.realms = " + newModel.realms);



            DataStore.i.realm.realmsInfo.Set(realms);

            Debug.Log("这里 realms = " + DataStore.i.realm.realmsInfo.Get());

            if (fetchRealmsTask != null)
            {
                fetchRealmsTask.TrySetResult(realms);
                fetchRealmsTask = null;
            }
        }

        public void SetAbout(string json)
        {
            JsonParser jsonParser = new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));
            AboutResponse aboutResponse = jsonParser.Parse<AboutResponse>(json);
            playerRealmAboutConfiguration.Set(aboutResponse.Configurations);
            playerRealmAboutContent.Set(aboutResponse.Content);
            playerRealmAboutLambda.Set(aboutResponse.Lambdas);
            realmName.Set(aboutResponse.Configurations.RealmName);
        }

        public UniTask<IReadOnlyList<RealmModel>> FetchRealmsInfo(CancellationToken cancellationToken)
        {
            try
            {
                if (fetchRealmsTask != null)
                    return fetchRealmsTask.Task.AttachExternalCancellation(cancellationToken)
                                          .Timeout(TimeSpan.FromSeconds(30));

                fetchRealmsTask = new UniTaskCompletionSource<IReadOnlyList<RealmModel>>();

                WebInterface.FetchRealmsInfo();

                return fetchRealmsTask.Task.AttachExternalCancellation(cancellationToken)
                                      .Timeout(TimeSpan.FromSeconds(30));
            }
            catch (Exception)
            {
                fetchRealmsTask = null;
                throw;
            }
        }
    }

    [Serializable]
    public class RealmsInfoModel
    {
        public CurrentRealmModel current;
        public RealmModel[] realms;
    }
}
