import api from "./api"

const getAllRoles = async () => {
    const response = await api.get(`/Role/all`);
    return response.data;

};

const getRoleById = async (id) => {
    const response = await api.get(`/Role/${id}`);
    return response.data;
};

const createRole = async (roleData) => {
    const response = await api.post(`/Role`, roleData);
    return response.data;
};

const updateRole = async (id, roleData) => {
    const response = await api.put(`/Role/${id}`, roleData);
    return response.data;
};

const deleteRole = async (id) => {
    await api.delete(`/Role/${id}`);
};

const roleService = {
    getAllRoles, 
    getRoleById, 
    createRole,
    updateRole,
    deleteRole,
};

export default roleService;