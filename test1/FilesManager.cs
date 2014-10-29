using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage;

namespace test1
{
    class FilesManager
    {
        public FilesManager()
        { }


        public void createJsonFile(String filename, gpsDataJasonTable[] arraydata)
        {
            //Get the isolated store of the app
            using (IsolatedStorageFile filefolder = IsolatedStorageFile.GetUserStoreForApplication())
            {   //Create the file
                using (IsolatedStorageFileStream streamtozip = new IsolatedStorageFileStream(filename, FileMode.Create, filefolder))

                //Save the data to the file
                using (StreamWriter sr = new StreamWriter(streamtozip))
                {
                    foreach (var item in arraydata)
                    {
                        sr.WriteLine(item.position);
                        //MessageBox.Show("guardando datos");
                    }
                }
            }
        }
        //ambos parametros son nombre de archivo, siempre trabajo en la local folder
        public void ZipFilesFromStorage(String jsonfilename, String filetozip)
        {
            using (IsolatedStorageFile filefolder = IsolatedStorageFile.GetUserStoreForApplication())
            {   //creates the zip file
                try
                {
                    
                        IsolatedStorageFileStream newZipFile = new IsolatedStorageFileStream(filetozip, FileMode.Create, filefolder);
                    //Creates the output of the compressed file
                    ZipOutputStream zipStream = new ZipOutputStream(newZipFile);
                    byte[] buffer = new byte[4096];
                    //search for the jsonfile to zip it
                    foreach (string fileName in filefolder.GetFileNames())
                    {
                        if (fileName == jsonfilename)
                        {
                            ZipEntry newEntry = new ZipEntry(fileName);

                            using (IsolatedStorageFileStream fileReader = new IsolatedStorageFileStream(fileName, FileMode.Open, FileAccess.Read, filefolder))
                            {
                                newEntry.Size = fileReader.Length;
                                zipStream.PutNextEntry(newEntry);
                                int sourceBytes;
                                do
                                {
                                    sourceBytes = fileReader.Read(buffer, 0, buffer.Length);
                                    zipStream.Write(buffer, 0, sourceBytes);

                                } while (sourceBytes > 0);
                            }
                            zipStream.Finish();
                            zipStream.Close();
                        }

                    }
                }

                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message,
                                      "Exception", MessageBoxButton.OKCancel);
                }
            }
        }

        public byte[] formatZipFileToSend(String zipfilename)
        {
            
            using (IsolatedStorageFile filestorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (filestorage.FileExists(zipfilename))
                {
                    IsolatedStorageFileStream fileStream = filestorage.OpenFile(zipfilename, FileMode.Open, FileAccess.Read);
                   
                        using (MemoryStream ms = new MemoryStream())
                        {
                            fileStream.CopyTo(ms);
                            //Cierro el archivo para poder eliminarlo despues
                            fileStream.Close();
                            return ms.ToArray();
                        }
                        
                 }
            }
            return null;
            

        }



        internal void deleteCreatedFiles(String zipfilename, String filename)
        {

            using (IsolatedStorageFile filestorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                   // StorageFolder folder = ApplicationData.Current.LocalFolder;
                    if (filestorage.FileExists(zipfilename))
                    {
                        
                        filestorage.DeleteFile(zipfilename);
                    }
                    if (filestorage.FileExists(filename))
                        filestorage.DeleteFile(filename);
                }
                catch (Exception exc){
                    MessageBox.Show(exc.Message, "Exception", MessageBoxButton.OK);
                }
            }

        }


    } //End namespace
} //End class




/*
 THIS CODE WAS THE TEST ONE BEFORE HAVING THIS CLASS, IT IS ONLY USEFUL FOR MY OWN REFERENCE
 * IF THIS COMMENT IS NOT DELETED IN THE FUTURE AND THE APP WORKS WELL, PLEASE FEEL FREE TO DELETE IT
 * 
 *  //Get all rows from the DB
            List<gpsDataJasonTable> data = dbjson.getAllRows();
            gpsDataJasonTable[] arraydata = data.ToArray();

            //Get the actual day, it will be the file name
            DateTime date = DateTime.Today;
            String filename = date.ToString("yyyyMMdd");
  
            // Create a new folder name DataFolder.
            // Get the local folder.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            //Create a file to store the arraydata data
            var file = await local.CreateFileAsync(filename,CreationCollisionOption.ReplaceExisting);
            //Save the data to the file
            using (StreamWriter sr = new StreamWriter(filename))
            {
                foreach (var item in arraydata)
                {
                    sr.WriteLine(item.position);
                    //MessageBox.Show("guardando datos");
                }
            }

           
            //Create the Zip file - has no name convention
            ZipFilesFromStorage(filename, "data.zip");
           

       //format the ZIP file to send it via http post   
         using (IsolatedStorageFile zipStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                  if (zipStorage.FileExists("data.zip"))
                   
                  { 
                               IsolatedStorageFileStream fileStream = zipStorage.OpenFile(path, FileMode.Open, FileAccess.Read);
                               
                               using (StreamReader reader = new StreamReader(fileStream))
                               {
                                   
                               }
                            //datatosend = JsonConvert.SerializeObject(filename);
                     //       byte[] b1 = System.Text.Encoding.UTF8.GetBytes(path);
                       
                               sendFileHttpPost();
                           }
                    }

 
 
 
 
 */