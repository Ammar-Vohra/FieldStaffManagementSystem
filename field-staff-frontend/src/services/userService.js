import api from "./api"

const getAllUsers = async () => {
    const response = await api.get(`/User/all`);
    return response.data;

};

const getUserById = async (id) => {
    const response = await api.get(`/User/${id}`);
    return response.data;
};

const getPaginatedUsers = async(params = {}) => {
    const response = await api.get(`/User`, {
        params: {
            pageNumber: params.pageNumber ?? 1,
            pageSize: params.pageSize ?? 10,
            search: params.search ?? "",
            sortBy: params.sortBy ?? "userName",
            sortDirection: params.sortDirection ?? "asc",
            isActive: params.isActive ?? undefined,
            roleId: params.roleId ?? undefined
        }
    });

    return response.data;
};

const createUser = async (userData) => {
    const response = await api.post(`/User`, userData);
    return response.data;
};

const updateUser = async (id, userData) => {
    const response = await api.put(`/User/${id}`, userData);
    return response.data;
};

const deleteUser = async (id) => {
    await api.delete(`/User/${id}`);
};

const userService = {
    getAllUsers, 
    getUserById,
    getPaginatedUsers, 
    createUser,
    updateUser,
    deleteUser,
};

export default userService;