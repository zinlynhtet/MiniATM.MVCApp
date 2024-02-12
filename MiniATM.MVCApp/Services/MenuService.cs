using MiniATM.MVCApp.Models;

namespace MiniATM.MVCApp.Services
{
    public class MenuService
    {
        public List<MenuModel> GenerateMenu(string user)
        {
            List<MenuModel> menuList = new List<MenuModel>();
           

            var roleCode = TblRole.Roles.FirstOrDefault(x => x.RoleName == user);
            var menuPermission = TblMenuPermission.MenuPermission
                .Where(x => x.RoleCode == roleCode.RoleCode).ToList();

            foreach (var item in menuPermission)
            {
                var menuHead = TblMenuHead.MenuHead
                    .FirstOrDefault(x => x.MenuCode == item.MenuCode
                                && x.RoleCode == item.RoleCode);
                var model = new MenuModel();
                model.MainMenu = menuHead.MenuName;
                model.ManinMenuController = menuHead.ControllerName;
                model.ActionName = menuHead.ActionName;
                if (menuHead.IsHasChild)
                {
                    var menuDetail = TblMenuDetail.MenuDetail
                        .Where(x => x.MenuParentCode == item.MenuCode
                                    && (x.RoleCode is null || x.RoleCode == roleCode.RoleCode))
                        .ToList();
                    foreach (var detail in menuDetail)
                    {
                        var detailModel = new SubMenuModel
                        {
                            SubMenu = detail.MenuChildName,
                            SubMenuController = detail.ControllerName,
                            ActionName = detail.ActionName
                        };
                        model.SubMenus.Add(detailModel);
                    }
                }
                menuList.Add(model);
            }

            return menuList;
        }
    }
}
