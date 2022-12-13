// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Web.Http;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Net;
using System.Reflection.PortableExecutable;
using Windows.Media.Playback;
using Windows.Media.Core;
using Windows.Media.Playlists;
using Windows.Storage;
using SocketIOClient;
using Windows.Web.Http.Filters;
using Microsoft.UI.Composition.Scenes;
using TestMusicStreamer.Classes;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TestMusicStreamer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Uri baseUri = new("http://localhost:9000");
        HttpBaseProtocolFilter filter = new();

        SocketIO socket;

        public MainWindow()
        {
            this.InitializeComponent();
            bool hasCookies = localSettings.Containers.ContainsKey("cookies");
            if (hasCookies)
            {
                //Cookies exist in settings, load them into our http clients
                foreach (var settings_cookie in localSettings.Containers["cookies"].Values)
                {
                    var cookie = new HttpCookie(settings_cookie.Key, baseUri.IdnHost, "/");
                    cookie.Value = settings_cookie.Value as string;

                    filter.CookieManager.SetCookie(cookie);
                }
            }
            
            Debug.WriteLine("COOKIE_MANAGER: " + filter.CookieManager.ToString());
        }

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Construct the HttpClient and Uri.
                var cookieManager = filter.CookieManager;
                var httpClient = new HttpClient(filter);
                var uri = new Uri(baseUri, "/api/auth");

                // Construct the JSON to post.
                HttpStringContent content = new HttpStringContent(
                    "{ \"username\": \"testboi\", \"password\": \"testpassword\" }",
                    UnicodeEncoding.Utf8,
                    "application/json");

                // Post the JSON and wait for a response.
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                    uri,
                    content);

                // write out the response.
                var httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();
                Debug.WriteLine(httpResponseBody);

                if (httpResponseMessage.StatusCode == Windows.Web.Http.HttpStatusCode.Ok)
                {
                    //HTTP OK means login was successful
                    //Store updated cookie information
                    bool hasCookies = localSettings.Containers.ContainsKey("cookies");
                    if (!hasCookies)
                    {
                        //No cookies in settings, so we need to make a container for them
                        localSettings.CreateContainer("cookies", ApplicationDataCreateDisposition.Always);
                    }
                    //Get rid of old cookie info
                    localSettings.Containers["cookies"].Values.Clear();
                    //Store new cookies
                    foreach (var cookie in cookieManager.GetCookies(baseUri))
                    {
                        localSettings.Containers["cookies"].Values[cookie.Name] = cookie.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                // Write out any exceptions.
                Debug.WriteLine(ex);
            }
            
        }

        private async void MediaPlayerInit_Click(object sender, RoutedEventArgs e)
        {
            MediaPlaybackList playlist = new MediaPlaybackList();

            // Construct the HttpClient and Uri.
            HttpClient httpClient = new HttpClient();;
            foreach (var entry in httpClient.DefaultRequestHeaders)
            {
                Debug.WriteLine("HEADER: " + entry.Key + ", VALUE: " + entry.Value);
            }

            Uri[] uris =
            {
                new Uri("http://localhost:9000/api/media/tracks/10/file"),
                new Uri("http://localhost:9000/api/media/tracks/9/file")
            };
            IEnumerable<HttpRandomAccessStream> sources = await Task.WhenAll(uris.Select(uri => HttpRandomAccessStream.CreateAsync(httpClient, uri).AsTask()));

            foreach (var src in sources)
            {
                playlist.Items.Add(new MediaPlaybackItem(MediaSource.CreateFromStream(src, src.ContentType)));
            }
            player.Source = playlist;
            
        }

        private void MediaPlayerDestroySrc_Test(object sender, RoutedEventArgs e)
        {
            MediaPlaybackList playlist = player.Source as MediaPlaybackList;
            playlist.Items.RemoveAt(0);
        }
    }
}
