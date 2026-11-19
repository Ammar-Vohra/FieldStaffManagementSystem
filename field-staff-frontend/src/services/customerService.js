import api from "./api";

const getAllCustomers = async () => {
  const response = await api.get("/Customers/all");
  return response.data;
};

const getPagedCustomers = async(params = {}) => {
    const response = await api.get(`/Customers`, {
        params: {
            pageNumber: params.pageNumber ?? 1,
            pageSize: params.pageSize ?? 10,
            search: params.search ?? "",
            sortBy: params.sortBy ?? "customerName",
            sortDirection: params.sortDirection ?? "asc",
            isActive: params.isActive ?? undefined,
            // roleId: params.roleId ?? undefined
        }
    });

    
    return response.data ?? response;
};

const getCustomerById = async (id) => {
    const response = await api.get(`/Customers/${id}`);
    return response.data;
};

const createCustomer = async (customerData) => {
    const response = await api.post(`/Customers`, customerData);
    return response.data;
};

const updateCustomer = async (id, customerData) => {
    const response = await api.put(`/Customers/${id}`, customerData);
    return response.data;
};

const deleteCustomer = async (id) => {
    const response = await api.delete(`/Customers/${id}`);
    return response.data;
}

const customerService = {
  getAllCustomers,
  getPagedCustomers,
  getCustomerById,
  createCustomer,
  updateCustomer,
  deleteCustomer,
};

export default customerService;