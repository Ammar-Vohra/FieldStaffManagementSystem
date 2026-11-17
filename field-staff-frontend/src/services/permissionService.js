import api from "./api"

const getAllPermissions = async () => {
    const response = await api.get(`/Permissions`);
    return response.data;
};

const getPermissionById = async (id) => {
    const response = await api.get(`/Permissions/${id}`);
    return response.data;

};

const createPermission = async (permissionData) => {
    const response = await api.post(`/Permissions`, permissionData);
    return response.data;
};

const updatePermission = async (id, permissionData) => {
    const response = await api.put(`/Permissions/${id}`, permissionData);
    return response.data;
};


const deletePermission = async (id) => {
    const response = await api.delete(`/Permissions/${id}`);
    return response.data;
};

const getUserPermissions = async (userId) => {
    const response = await api.get(
        `/UserPermissions/user/${userId}`
    );

    return response.data;
};


const updateUserPermissions = async (userId, permissions) => {
    const response = await api.put(
        `/UserPermissions/user/${userId}`,
        {
            permissions: permissions.map(permissionId => ({
                permissionId,
                expiresAt: null
            }))
        }
    );

    return response.data;
};


const permissionService = {
    getAllPermissions,
    getPermissionById,
    createPermission,
    updatePermission,
    deletePermission,
    getUserPermissions,
    updateUserPermissions
};


export default permissionService;