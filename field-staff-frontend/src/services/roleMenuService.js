import api from "./api"

const getMyMenu = async () => {
    const response = await api.get("/RoleMenus/my-menus");
    return response.data;
} 

const roleMenuService = {getMyMenu,};

export default roleMenuService;