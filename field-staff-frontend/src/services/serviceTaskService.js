import api from "./api";

const getPaginatedServiceTasks = async (params = {}) => {
  const response = await api.get("/ServiceTask", {
    params: {
      pageNumber: params.pageNumber ?? 1,
      pageSize: params.pageSize ?? 10,
      search: params.search || undefined,
      sortBy: params.sortBy ?? "scheduledDate",
      sortDirection: params.sortDirection ?? "desc",
      customerId: params.customerId || undefined,
      assignedStaffId: params.assignedStaffId || undefined,
      taskStatusId: params.taskStatusId || undefined,
      taskPriorityId: params.taskPriorityId || undefined,
      scheduledDateFrom: params.scheduledDateFrom || undefined,
      scheduledDateTo: params.scheduledDateTo || undefined,
      isActive: params.isActive ?? undefined,
    },
  });

  return response.data;
};

const getServiceTaskById = async (id) => {
  const response = await api.get(`/ServiceTask/${id}`);
  return response.data;
};

const createServiceTask = async (taskData) => {
  const response = await api.post("/ServiceTask", taskData);
  return response.data;
};

const updateServiceTask = async (id, taskData) => {
  const response = await api.put(`/ServiceTask/${id}`, taskData);
  return response.data;
};

const deleteServiceTask = async (id) => {
  const response = await api.delete(`/ServiceTask/${id}`);
  return response.data;
};

const serviceTaskService = {
  getPaginatedServiceTasks,
  getServiceTaskById,
  createServiceTask,
  updateServiceTask,
  deleteServiceTask,
};

export default serviceTaskService;