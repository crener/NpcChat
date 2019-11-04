using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using NpcChat.Views.Utility;
using NpcChatSystem.Utilities;
using Prism.Commands;
using LogLevel = NLog.LogLevel;

namespace NpcChat.Views.About
{
    public class LicensePageVM
    {
        public ObservableCollection<LicenseData> Licenses { get; } = new ObservableCollection<LicenseData>();
        public ICommand OpenLink { get; }

        public LicensePageVM()
        {
            OpenLink = new DelegateCommand<string>(SimpleHelpers.OpenLink);
            LoadLicenses(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Licenses", "LicenseKey.xml"));
        }

        private void LoadLicenses(string licenseFile)
        {
            bool createPlaceholder = false;
            string errorDescription = "No licenses found in license file!";

            if (File.Exists(licenseFile))
            {
                try
                {
                    XDocument root = XDocument.Load(licenseFile);
                    if (root?.Root == null)
                    {
                        errorDescription = $"No Data extracted from license file!";
                        createPlaceholder = true;
                    }
                    else
                    {
                        string licenseDirectory = Path.GetDirectoryName(licenseFile);
                        foreach (XElement license in root.Root.Elements("License"))
                        {
                            LicenseData licenseData = new LicenseData();
                            licenseData.Name = license.Attribute("Name")?.Value ?? "No License File";
                            licenseData.Link = license.Attribute("Link")?.Value ?? "...";

                            const string licenseFileName = "licenseFile";
                            if (license.Attribute(licenseFileName) != null)
                            {
                                string builtLicenseFile = Path.Combine(licenseDirectory, license.Attribute(licenseFileName).Value);
                                if (File.Exists(builtLicenseFile)) licenseData.License = File.ReadAllText(builtLicenseFile);
                                else licenseData.License = $"License file not found! file '{builtLicenseFile}'";
                            }
                            else licenseData.License = "No license file defined!";

                            Licenses.Add(licenseData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logger.Log(LogLevel.Error, $"Failed to extact License data from '{licenseFile}'", ex);

                    errorDescription = $"Failed to load data from the license file '{licenseFile}'\n\nReason: {ex.Message}";
                    createPlaceholder = true;
                }
            }
            else
            {
                createPlaceholder = true;
                errorDescription = $"Failed to find the license file '{licenseFile}'";
            }

            if (createPlaceholder || Licenses.Count == 0)
            {
                Licenses.Clear();

                LicenseData licenseData = new LicenseData();
                licenseData.Name = "No License File";
                licenseData.Link = "...";
                licenseData.License = $"Oh No! :(\n\n{errorDescription}";

                Licenses.Add(licenseData);
            }
        }
    }

    public class LicenseData
    {
        public string Name { get; set; } = "Unknown";
        public string Link { get; set; }
        public string License { get; set; }
    }
}
