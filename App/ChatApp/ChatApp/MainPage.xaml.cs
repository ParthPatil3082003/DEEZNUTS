using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ChatApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>



    public sealed partial class MainPage : Page
    {
        string FileName = "DemoMessage.txt";
        public async void ReadFromTextFile()
        {
            
            string Content = "";
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///" + FileName));
            using (StreamReader sr = new StreamReader(await file.OpenStreamForReadAsync()))
            {
                Content = sr.ReadToEnd();
            }

           /* MessageDialog msg = new MessageDialog(Content);
            await msg.ShowAsync();
           */


        }


        public static class FileHelper
        {

            // Write a text file to the app's local folder. 

            public static async Task<string>
               WriteTextFile(string filename, List<string> AOC)
            {
                

                // Create a StorageFolder object for the Documents folder.
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                // Create a StorageFile object for the file you want to append to.
                StorageFile textFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);

                // Use the FileIO.AppendTextAsync method to append the text to the file.
                await FileIO.AppendLinesAsync(textFile, AOC);
                //await File.AppendAllTextAsync(filename,contents);

                //  StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                //StorageFile textFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

              /*  using (IRandomAccessStream textStream = await textFile.OpenAsync(FileAccessMode.ReadWrite))
                {

                    using (DataWriter textWriter = new DataWriter(textStream))
                    {
                        textWriter.WriteString(contents);
                        await textWriter.StoreAsync();
                    }
                }*/

                
                return textFile.Path;
                
            }

            // Read the contents of a text file from the app's local folder.

            public static async Task<string> ReadTextFile(string filename)
            {
                string contents;
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile textFile = await localFolder.GetFileAsync(filename);

                using (IRandomAccessStream textStream = await textFile.OpenReadAsync())
                {

                    using (DataReader textReader = new DataReader(textStream))
                    {
                        uint textLength = (uint)textStream.Size;
                        await textReader.LoadAsync(textLength);
                        contents = textReader.ReadString(textLength);
                    }

                }

                return contents;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            ReadFromTextFile();
           
            
        }

        private void Contacts_Click(object sender, RoutedEventArgs e)
        {
            Contacts.IsPaneOpen = !Contacts.IsPaneOpen;


        }

        public void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (User1.IsSelected) { UserName.Text = "Mayuresh"; FileName = "Mayuresh"; }
            else if (User2.IsSelected) { UserName.Text = "Kshitij"; FileName = "Kshitij"; }
            else if (User3.IsSelected) { UserName.Text = "Sarthak"; FileName = "Sarthak"; }
        }

        public async void Send_Click(object sender, RoutedEventArgs e)
        {
           // string FileName = "";
            string Content = "";
            List<string> M = new List<string> { };
            string s1=null;


            List<string> AOC = new List<string> { "You: " + MessageBox.Text };
            
            
            
            
            
            string textFilePath = await FileHelper.WriteTextFile(FileName, AOC);
            MessageBox.Text = "";

            // Create a StorageFolder object for the Documents folder.
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            // Create a StorageFile object for the file you want to append to.
            StorageFile textFile = await localFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);


            //   StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///" + FileName));
            using (StreamReader sr = new StreamReader(await textFile.OpenStreamForReadAsync()))
            {
                int lineCount = 0;
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    s1 = line;

                    M.Add(s1);

                    lineCount++;

                }
                //  Content = sr.ReadToEnd();
                /*  while ((Content = sr.ReadLine()) != null)
                 {
                    Int.Items.Add(Content);
                 }*/
            }

            //string str1 = await FileHelper.ReadTextFile(FileName);


            List<string> str = new List<string>
                {
                   await FileHelper.ReadTextFile(FileName)

                };
                myListView.ItemsSource = M;

            
            //Chat.Items.Add(str);
            // Int.Items.Add(textFilePath);

            // Int.Items.Add(Content);


        }

        private void MessageBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Send_Click(sender, e);
                e.Handled = true;
            }
        }
    }
}
