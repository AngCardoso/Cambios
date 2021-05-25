using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cambios.Servicos
{
    class DialogService
    {
        public void ShowMessage (string Title, string Message)
        {
            MessageBox.Show(Message, Title);
        }
    }
}
