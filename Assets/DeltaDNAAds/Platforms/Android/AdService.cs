//
// Copyright (c) 2016 deltaDNA Ltd. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace DeltaDNAAds.Android
{
    #if UNITY_ANDROID
    internal class AdService : ISmartAdsManager
    {
        private readonly IDictionary<string, AndroidJavaObject> engageListeners;
        private readonly AndroidJavaObject activity;
        private AndroidJavaObject adService;

        internal AdService(DDNASmartAds ads) {
            engageListeners = new Dictionary<string, AndroidJavaObject>();

            try {
                activity = new AndroidJavaClass(Utils.UnityActivityClassName).GetStatic<AndroidJavaObject>("currentActivity");
                adService = new AndroidJavaObject(Utils.AdServiceWrapperClassName).CallStatic<AndroidJavaObject>(
                    "create", activity, new AdServiceListener(ads, engageListeners));
            } catch (AndroidJavaException exception) {
                DeltaDNA.Logger.LogDebug("Exception creating Android AdService: "+exception.Message);
                throw new System.Exception("Native Android SmartAds AAR not found.");
            }
        }

        public void RegisterForAds(string decisionPoint)
        {
            adService.Call("init", decisionPoint);
        }

        public bool IsInterstitialAdAvailable() {
            return adService.Call<bool>("isInterstitialAdAvailable");
        }

        public void ShowInterstitialAd() {
            ShowInterstitialAd(null);
        }

        public void ShowInterstitialAd(string decisionPoint) {
            adService.Call("showInterstitialAd", decisionPoint);
        }

        public bool IsRewardedAdAvailable() {
            return adService.Call<bool>("isRewardedAdAvailable");
        }

        public void ShowRewardedAd() {
            ShowRewardedAd(null);
        }

        public void ShowRewardedAd(string decisionPoint) {
            adService.Call("showRewardedAd", decisionPoint);
        }

        public void EngageResponse(string id, string response, int statusCode, string error) {
            // android sdk expects request listener callbacks on the main thread
            activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                AndroidJavaObject listener;
                if (engageListeners.TryGetValue(id, out listener)) {
                    if (statusCode >= 200 && statusCode < 300) {
                        listener.Call("onSuccess", new AndroidJavaObject(Utils.JSONObjectClassName, response));
                    } else {
                        listener.Call("onFailure", new AndroidJavaObject(Utils.ThrowableClassName, error));
                    }

                    engageListeners.Remove(id);
                }
            }));
        }

        public void OnPause()
        {
            adService.Call("onPause");
        }

        public void OnResume()
        {
            adService.Call("onResume");
        }

        public void OnDestroy()
        {
            adService.Call("onDestroy");
        }
    }
    #endif
}