﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Windows.Toolkit.Notifications;
using Microsoft.Windows.Toolkit.SampleApp.Models;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.System.Profile;
using Windows.Foundation.Metadata;
using Windows.UI.StartScreen;

namespace Microsoft.Windows.Toolkit.SampleApp.SamplePages
{
    public sealed partial class WeatherLiveTileAndToastPage : Page
    {
        public WeatherLiveTileAndToastPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var propertyDesc = e.Parameter as PropertyDescriptor;

            if (propertyDesc != null)
            {
                DataContext = propertyDesc.Expando;
            }
        }

        private void ButtonPinTile_Click(object sender, RoutedEventArgs e)
        {
            PinTile();
        }

        private async void PinTile()
        {
            SecondaryTile tile = new SecondaryTile(DateTime.Now.Ticks.ToString(), "WeatherSample", "args", new Uri("ms-appx:///Assets/Square150x150Logo.png"), TileSize.Default);

            if (!await tile.RequestCreateAsync())
            {
                return;
            }

            TileContent content = GenerateTileContent();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content.GetContent());
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId).Update(new TileNotification(doc));
        }

        private void ButtonPopToast_Click(object sender, RoutedEventArgs e)
        {
            PopToast();
        }

        private void PopToast()
        {
            ToastContent content = GenerateToastContent();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content.GetContent());
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(doc));
        }

        public static ToastContent GenerateToastContent()
        {
            // Start by constructing the visual portion of the toast
            ToastBindingGeneric binding = new ToastBindingGeneric();

            // We'll always have this summary text on our toast notification
            // (it is required that your toast starts with a text element)
            binding.Children.Add(new AdaptiveText()
            {
                Text = "Today will be mostly sunny with a high of 63 and a low of 42."
            });

            // If Adaptive Toast Notifications are supported
            if (IsAdaptiveToastSupported())
            {
                // Use the rich Tile-like visual layout
                binding.Children.Add(new AdaptiveGroup()
                {
                    Children =
                    {
                        GenerateSubgroup("Mon", "Mostly Cloudy.png", 63, 42),
                        GenerateSubgroup("Tue", "Cloudy.png", 57, 38),
                        GenerateSubgroup("Wed", "Sunny.png", 59, 43),
                        GenerateSubgroup("Thu", "Sunny.png", 62, 42),
                        GenerateSubgroup("Fri", "Sunny.png", 71, 66)
                    }
                });
            }

            // Otherwise...
            else
            {
                // We'll just add two simple lines of text
                binding.Children.Add(new AdaptiveText()
                {
                    Text = "Monday ⛅ 63° / 42°"
                });

                binding.Children.Add(new AdaptiveText()
                {
                    Text = "Tuesday ☁ 57° / 38°"
                });
            }

            // Construct the entire notification
            return new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    // Use our binding from above
                    BindingGeneric = binding,

                    // Set the base URI for the images, so we don't redundantly specify the entire path
                    BaseUri = new Uri("Assets/NotificationAssets/", UriKind.Relative)
                },

                // Include launch string so we know what to open when user clicks toast
                Launch = "action=viewForecast&zip=98008"
            };
        }

        private static bool IsAdaptiveToastSupported()
        {
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                // Desktop and Mobile started supporting adaptive toasts in API contract 3 (Anniversary Update)
                case "Windows.Mobile":
                case "Windows.Desktop":
                    return ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3);

                // Other device families do not support adaptive toasts
                default:
                    return false;
            }
        }

        public static TileContent GenerateTileContent()
        {
            return new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileSmall = GenerateTileBindingSmall(),
                    TileMedium = GenerateTileBindingMedium(),
                    TileWide = GenerateTileBindingWide(),
                    TileLarge = GenerateTileBindingLarge(),

                    // Set the base URI for the images, so we don't redundantly specify the entire path
                    BaseUri = new Uri("Assets/NotificationAssets/", UriKind.Relative)
                }
            };
        }

        private static TileBinding GenerateTileBindingSmall()
        {
            return new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {
                    TextStacking = TileTextStacking.Center,

                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = "Mon",
                            HintStyle = AdaptiveTextStyle.Body,
                            HintAlign = AdaptiveTextAlign.Center
                        },

                        new AdaptiveText()
                        {
                            Text = "63°",
                            HintStyle = AdaptiveTextStyle.Base,
                            HintAlign = AdaptiveTextAlign.Center
                        }
                    }
                }
            };
        }

        private static TileBinding GenerateTileBindingMedium()
        {
            return new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {
                    Children =
                    {
                        new AdaptiveGroup()
                        {
                            Children =
                            {
                                GenerateSubgroup("Mon", "Mostly Cloudy.png", 63, 42),
                                GenerateSubgroup("Tue", "Cloudy.png", 57, 38)
                            }
                        }
                    }
                }
            };
        }

        private static TileBinding GenerateTileBindingWide()
        {
            return new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {
                    Children =
                    {
                        new AdaptiveGroup()
                        {
                            Children =
                            {
                                GenerateSubgroup("Mon", "Mostly Cloudy.png", 63, 42),
                                GenerateSubgroup("Tue", "Cloudy.png", 57, 38),
                                GenerateSubgroup("Wed", "Sunny.png", 59, 43),
                                GenerateSubgroup("Thu", "Sunny.png", 62, 42),
                                GenerateSubgroup("Fri", "Sunny.png", 71, 66)
                            }
                        }
                    }
                }
            };
        }

        private static TileBinding GenerateTileBindingLarge()
        {
            return new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {
                    Children =
                    {
                        new AdaptiveGroup()
                        {
                            Children =
                            {
                                new AdaptiveSubgroup()
                                {
                                    HintWeight = 30,
                                    Children =
                                    {
                                        new AdaptiveImage() { Source = "Mostly Cloudy.png" }
                                    }
                                },

                                new AdaptiveSubgroup()
                                {
                                    Children =
                                    {
                                        new AdaptiveText()
                                        {
                                            Text = "Monday",
                                            HintStyle = AdaptiveTextStyle.Base
                                        },

                                        new AdaptiveText()
                                        {
                                            Text = "63° / 42°"
                                        },

                                        new AdaptiveText()
                                        {
                                            Text = "20% chance of rain",
                                            HintStyle = AdaptiveTextStyle.CaptionSubtle
                                        },

                                        new AdaptiveText()
                                        {
                                            Text = "Winds 5 mph NE",
                                            HintStyle = AdaptiveTextStyle.CaptionSubtle
                                        }
                                    }
                                }
                            }
                        },

                        // For spacing
                        new AdaptiveText(),

                        new AdaptiveGroup()
                        {
                            Children =
                            {
                                GenerateLargeSubgroup("Tue", "Cloudy.png", 57, 38),
                                GenerateLargeSubgroup("Wed", "Sunny.png", 59, 43),
                                GenerateLargeSubgroup("Thu", "Sunny.png", 62, 42),
                                GenerateLargeSubgroup("Fri", "Sunny.png", 71, 66)
                            }
                        }
                    }
                }
            };
        }

        private static AdaptiveSubgroup GenerateSubgroup(string day, string img, int tempHi, int tempLo)
        {
            return new AdaptiveSubgroup()
            {
                HintWeight = 1,

                Children =
                {
                    // Day
                    new AdaptiveText()
                    {
                        Text = day,
                        HintAlign = AdaptiveTextAlign.Center
                    },

                    // Image
                    new AdaptiveImage()
                    {
                        Source = img,
                        HintRemoveMargin = true
                    },

                    // High temp
                    new AdaptiveText()
                    {
                        Text = tempHi + "°",
                        HintAlign = AdaptiveTextAlign.Center
                    },

                    // Low temp
                    new AdaptiveText()
                    {
                        Text = tempLo + "°",
                        HintAlign = AdaptiveTextAlign.Center,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    }
                }
            };
        }

        private static AdaptiveSubgroup GenerateLargeSubgroup(string day, string image, int high, int low)
        {
            // Generate the normal subgroup
            var subgroup = GenerateSubgroup(day, image, high, low);

            // Allow there to be padding around the image
            (subgroup.Children[1] as AdaptiveImage).HintRemoveMargin = null;

            return subgroup;
        }

        private static AdaptiveText GenerateLegacyToastText(string day, string weatherEmoji, int tempHi, int tempLo)
        {
            return new AdaptiveText()
            {
                Text = $"{day} {weatherEmoji} {tempHi}° / {tempLo}°"
            };
        }
    }
}