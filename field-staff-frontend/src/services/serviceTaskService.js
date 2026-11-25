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


const getMyTasks = async (params = {}) => {
  const response = await api.get("/ServiceTask/MyTasks", {
    params: {
      pageNumber: params.pageNumber ?? 1,
      pageSize: params.pageSize ?? 10,
      search: params.search || undefined,
      sortBy: params.sortBy ?? "scheduledDate",
      sortDirection: params.sortDirection ?? "desc",
      taskStatusId: params.taskStatusId || undefined,
      taskPriorityId: params.taskPriorityId || undefined,
      scheduledDateFrom: params.scheduledDateFrom || undefined,
      scheduledDateTo: params.scheduledDateTo || undefined,
      isActive: params.isActive ?? undefined,
    },
  });

  return response.data;
};


const getMyTaskDetails = async (id) => {
  const response = await api.get(`/ServiceTask/MyTasks/${id}`);
  return response.data;
};

const updateMyTaskStatus = async (id, data) => {
  const response = await api.put(
    `/ServiceTask/MyTasks/${id}/Status`,
    data
  );

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

const reassignServiceTask = async (id, assignmentData) => {
  const response = await api.put(`/ServiceTask/${id}/assign`, assignmentData);
  return response.data;
};

const deleteServiceTask = async (id) => {
  const response = await api.delete(`/ServiceTask/${id}`);
  return response.data;
};

const serviceTaskService = {
  getPaginatedServiceTasks,
  getMyTasks,
  getMyTaskDetails,
  updateMyTaskStatus,
  getServiceTaskById,
  createServiceTask,
  updateServiceTask,
  reassignServiceTask,
  deleteServiceTask,
};

export default serviceTaskService;
