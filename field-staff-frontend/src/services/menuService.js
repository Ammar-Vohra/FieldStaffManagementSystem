import api from "./api"

const getAllMenus = async () => {
    const response = await api.get(`/Menus`);
    return response.data;

};

const getPaginatedMenus =  async(params = {}) => {
    const response = await api.get(`/Menus`, {
        params: {
            pageNumber: params.pageNumber ?? 1,
            pageSize: params.pageSize ?? 10,
            search: params.search ?? "",
            sortBy: params.sortBy ?? "menuName",
            sortDirection: params.sortDirection ?? "asc",
            isActive: params.isActive ?? undefined,
            parentMenuId: params.parentMenuId ?? undefined
        }
    });

    return response.data;
};

const getMenuById = async (id) => {
    const response = await api.get(`/Menus/${id}`);
    return response.data;
};

const createMenu = async (menuData) => {
    const response = await api.post(`/Menus`, menuData);
    return response.data;
};

const updateMenu = async (id, menuData) => {
    const response = await api.put(`/Menus/${id}`, menuData);
    return response.data;
};

const deleteMenu = async (id) => {
    await api.delete(`/Menus/${id}`);
};

const menuService = {
    getAllMenus, 
    getMenuById, 
    getPaginatedMenus,
    createMenu,
    updateMenu,
    deleteMenu,
};

export default menuService;