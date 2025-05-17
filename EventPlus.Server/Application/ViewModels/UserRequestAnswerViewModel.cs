using System;

namespace EventPlus.Server.Application.ViewModels
{
    public class UserRequestAnswerViewModel
    {
        public int IdUserRequestAnswer { get; set; }
        public string Answer { get; set; }
        public int FkQuestionidQuestion { get; set; }
        public int? FkUseridUser { get; set; }  // Vartotojo ID, kuris pateikė užklausą
        public int? FkAdministratoridUser { get; set; } // Administratoriaus ID, kuris tvarkė užklausą
        public int? FkOrganiseridUser { get; set; } // Organizatoriaus ID, jei reikalingas
    }
}