using System;
using System.IO;

namespace FolderSync__ver2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please insert Source folder path:");
            string location1 = Console.ReadLine();
            Console.WriteLine("Please insert Replica folder path:");
            string location2 = Console.ReadLine();

            if (location1 == location2)                                                                 //Dummy test
            {
                Console.WriteLine("Unfortunately, you entered same path. Please start again");
                Console.WriteLine("Press enter to restart");
                Console.ReadLine();
                Console.Clear();
                Main(args);
            }


            Console.WriteLine("Folders are synchronized!");
            System.Threading.Thread.Sleep(1000);
            Console.Clear();
            while (true)
            {
                SyncDir(location1, location2);
                System.Threading.Thread.Sleep(100);                                      //To aimulate simultanious synchronization 100ms stops had to be added
            }
        }

        public static void SyncDir(string Source, string Replica)
        {

            Directory.CreateDirectory(Replica);

            foreach (string source in Directory.GetDirectories(Source))
            {
                try { SyncDir(source, Replica + "/" + Path.GetFileName(source)); }
                catch (Exception ex)
                {
                    if (ex is DirectoryNotFoundException || ex is FileNotFoundException) { }
                    throw;
                }
            }
            //--------------------------------------------------------------------------------------------------------------------------------------
            foreach (string S in Directory.GetFiles(Source))
            {
                string C = Replica + "/" + Path.GetFileName(S);
                System.Threading.Thread.Sleep(10);
                if (!File.Exists(C))
                {
                    try { File.Copy(S, C); }
                    catch (System.IO.FileNotFoundException) { }                        //Unfortunately in these 100ms window program might encounter some problems
                    catch (System.IO.DirectoryNotFoundException) { }                    //and for this situation try/catch was used.
                }
                else
                {
                    FileInfo fi1 = new FileInfo(S);
                    FileInfo fi2 = new FileInfo(C);
                    if (fi1.LastWriteTime != fi2.LastWriteTime)                          //This lines of code are responsible for updating the files
                    {                                                                    //The modification of data inside the file or simply renaming the file 
                        File.Delete(C);
                        try { File.Copy(S, C); }
                        catch (System.IO.FileNotFoundException) { }
                        catch (System.IO.DirectoryNotFoundException) { }
                        Console.WriteLine("Update file {0} from file {1}", S, C);
                    }
                }
            }
            //----------------------------------------------------------------------------------------------------------------------------------------------

            foreach (string rFile in Directory.GetFiles(Replica))                                  //This lines are responsible for the folder deletion//
            {
                string sFile = Source + "/" + Path.GetFileName(rFile);
                if (!File.Exists(sFile))
                {
                    FileDeletion(Source, Replica);
                }
            }
            foreach (string rDir in Directory.GetDirectories(Replica))
            {
                string sDir = Source + "/" + Path.GetDirectoryName(rDir);                         //This lines are responsible for the file deletion//
                if (!Directory.Exists(sDir))
                {
                    FolderDeletion(Source, Replica);
                }
            }
            //----------------------------------------------------------------------------------------------------------------------------------------------
        }
        public static void FileDeletion(string fdSource, string fdReplica)                       //File deletion function//
        {

            string fdFile_S = fdSource + "/" + Path.GetFileName(fdReplica);
            foreach (string fdFile_R in Directory.GetFiles(fdReplica))
            {
                if (!File.Exists(fdFile_S))
                {
                    try
                    {
                        File.Delete(fdFile_R);
                        Console.Clear();
                        Console.WriteLine("{0} Files were deleted", fdFile_R);
                    }
                    catch (System.IO.IOException) { Console.WriteLine("Catched on deletion of file"); }
                }
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------------
        public static void FolderDeletion(string ddSource, string ddReplica)            //Folder deletion function//
        {

            foreach (string ddDir_R in Directory.GetDirectories(ddReplica))
            {
                string ddDir_S = ddSource + "/" + Path.GetFileName(ddDir_R);
                                                                                        //The recursion has been made to delete files first, then folder
                if (!Directory.Exists(ddDir_S))                                         //It is however possible to do so if we add boolean inside Directory.Delete()
                {                                                                       //However it let's program to contionuosly delete and insert folders. Making work on destination
                    Console.Clear();                                                    //One issue with folder deletion is that if they are not empty, it is not possible to delete them
                    Console.WriteLine("Deleting folders: {0}", ddDir_R);                //folder impossible. 
                    FileDeletion(ddDir_S, ddDir_R);                                       
                    FolderDeletion(ddDir_S, ddDir_R);
                    Directory.Delete(ddDir_R);
                }
            }
        }
    }
}
