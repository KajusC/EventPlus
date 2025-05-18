using System;

namespace EventPlus.Server.Application.ViewModels
{
    public class QuestionViewModel
    {
        public int IdQuestion { get; set; }
        public string FormulatedQuestion { get; set; }
        public int? FkAdministratoridUser { get; set; }
        public string AdministratorName { get; set; }
    }
}