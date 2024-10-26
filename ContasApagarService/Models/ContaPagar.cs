using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContasApagarService.Models
{
    public class ContaPagar
    {
        public int Id { get; set; }
        public string? Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
    }
}