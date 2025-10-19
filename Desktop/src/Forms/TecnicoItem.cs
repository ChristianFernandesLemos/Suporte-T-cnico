using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaChamados.Controllers;
using SistemaChamados.Models;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaChamados.Forms
{
    /// <summary>
    /// Classe auxiliar para itens de técnicos no ComboBox
    /// </summary>
    public class TecnicoItem
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public override string ToString()
        {
            return Nome;
        }
    }
}
