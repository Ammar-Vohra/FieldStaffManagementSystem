import api from "./api"

const getMyMenu = async () => {
    const response = await api.get("/RoleMenus/my-menus");
    return response.data;
};

const getAllMenus = async () => {
    const response = await api.get(`/RoleMenus`);
    return response.data;
};

const getRoleMenuById = async (roleMenuId) => {
    const response = await api.get(`/RoleMenus/${roleMenuId}`);
    return response.data;

};

const createRoleMenuById = async (roleMenuData) => {
    const response = await api.post(`/RoleMenus`,roleMenuData);
    return response.data;

};

const updateRoleMenuById = async (roleMenuId, roleMenuData) => {
    const response = await api.put(`/RoleMenus/${roleMenuId}`, roleMenuData);
    return response.data;

};

const deleteRoleMenuById = async (roleMenuId) => {
    const response = await api.delete(`/RoleMenus/${roleMenuId}`);
    return response.data;

};

const roleMenuService = {
    getMyMenu,
    getAllMenus,
    getRoleMenuById,
    createRoleMenuById,
    updateRoleMenuById,
    deleteRoleMenuById,
};

export default roleMenuService;

