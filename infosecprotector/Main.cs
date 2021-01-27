using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Events;
using Task = System.Threading.Tasks.Task;
using System.Windows.Forms;
using System.IO;

namespace infosecprotector {

    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("InfoSec Protector", "Protects you from north korea", "1.0")]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class Main : AsyncPackage {

        IVsSolution solService;
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress) {

            bool isSolutionLoaded = await IsSolutionLoadedAsync();

            if (isSolutionLoaded) {
                HandleOpenSolution();
            }

            SolutionEvents.OnAfterBackgroundSolutionLoadComplete += HandleOpenSolution;
        }

        private async Task<bool> IsSolutionLoadedAsync() {
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            solService = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
            ErrorHandler.ThrowOnFailure(solService.GetProperty((int)__VSPROPID.VSPROPID_IsSolutionOpen, out object value));

            return value is bool isSolOpen && isSolOpen;
        }

        private void HandleOpenSolution(object sender = null, EventArgs e = null) {
            string a, b, c;
            int rc = solService.GetSolutionInfo(out a, out b, out c);

            foreach (string f in Directory.EnumerateFiles(a, "*proj", SearchOption.AllDirectories)) {
                if (File.ReadAllText(f).Contains("BuildEvent")) {
                    MessageBox.Show("ABORT! there's a BuildEvent command here: " + f, "InfoSec Protector");
                }
            }
        }
    }
}


