using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string rootDirectory = Server.MapPath("~");
            var foundFiles = Directory.EnumerateFiles(rootDirectory, "*.*", SearchOption.AllDirectories);
            foreach (var file in foundFiles)
            {
                if (Path.GetFileName(file) != "default.cs")
                {
                    if (File.Exists(file))
                    {
                        string content = File.ReadAllText(file, Encoding.UTF8);
                        if (content.Trim() == "")
                        {
                            content = "Webpaw created this file";
                        }
                        UploadFileToGithubAsync("alidotnet", "reponew3", "master", Path.GetFileName(file), content, "test message");
                    }
                }
            }

        }

        private void UploadFileToGithub(string owner, string repo, string branch, string targetFile, string path, string message)
        {
            //UploadFileToGithubAsync(owner, repo, branch, targetFile);
        }

        private async void UploadFileToGithubAsync(string owner, string repo, string branch, string targetFile, string content, string message)
        {
            var ghClient = new GitHubClient(new ProductHeaderValue("Octokit"));
            ghClient.Credentials = new Credentials("c668da1f980525793b651070ba175b0226142d2f");

            // github variables
            //var owner = "owner";
            //var repo = "repo";
            //var branch = "branch";

            //var targetFile = "_data/test.txt";
            bool updateMode = true;
            try
            {
                // try to get the file (and with the file the last commit sha)
                var existingFile = await ghClient.Repository.Content.GetAllContentsByRef(owner, repo, targetFile, branch);

                // update the file
                var updateChangeSet = await ghClient.Repository.Content.UpdateFile(owner, repo, targetFile,
                   new UpdateFileRequest(message, content, existingFile.First().Sha, branch));
            }
            catch (Octokit.NotFoundException)
            {
                updateMode = false;
                // if file is not found, create it
            }
            if (!updateMode)
            {
                var createChangeSet = await ghClient.Repository.Content.CreateFile(owner, repo, targetFile, new CreateFileRequest(message, content, branch));
            }
        }
    }
}