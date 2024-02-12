namespace MiniATM.MVCApp.Models
{
    public class MenuModel
    {
        public string MainMenu { get; set; }
        public string ManinMenuController { get; set; }
        public string ActionName { get; set; }
        public List<SubMenuModel> SubMenus { get; set; } = new List<SubMenuModel>();

    }

    public class SubMenuModel
    {
        public string SubMenu { get; set; }
        public string SubMenuController { get; set;}
        public string ActionName { get; set; }
    }
}
