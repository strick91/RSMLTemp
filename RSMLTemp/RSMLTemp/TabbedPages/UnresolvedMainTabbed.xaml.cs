using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RSMLTemp.TabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnresolvedMainTabbed : TabbedPage
    {
        public UnresolvedMainTabbed()
        {
            InitializeComponent();
        }
    }
}