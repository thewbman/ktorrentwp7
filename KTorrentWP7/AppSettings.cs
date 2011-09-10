﻿using System;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Collections.Generic;

namespace KTorrentWP7
{
    public class AppSettings
    {
        // Our isolated storage settings
        IsolatedStorageSettings settings;

        // The isolated storage key names of our settings
        const string ThemeKeyName = "Theme";
        const string AnalyticsKeyName = "Analytics";

        // The default value of our settings
        const string ThemeDefault = "blue";
        const bool AnalyticsDefault = true;

        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        public AppSettings()
        {
            // Get the settings for this application.
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            settings.Save();
        }


        /// <summary>
        /// Property to get and set a Theme Setting Key.
        /// </summary>
        public string ThemeSetting
        {
            get
            {
                return GetValueOrDefault<string>(ThemeKeyName, ThemeDefault);
            }
            set
            {
                if (AddOrUpdateValue(ThemeKeyName, value))
                {
                    Save();
                }
            }
        }


        /// <summary>
        /// Property to get and set a Analtyics Setting Key.
        /// </summary>
        public bool AnaltyicsSetting
        {
            get
            {
                return GetValueOrDefault<bool>(AnalyticsKeyName, AnalyticsDefault);
            }
            set
            {
                if (AddOrUpdateValue(AnalyticsKeyName, value))
                {
                    Save();
                }
            }
        }


    }
}