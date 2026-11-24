import api from "./api";

const getAllTaskStatuses = async () => {
  const response = await api.get("/TaskStatus/all");
  return response.data;
};


const createTaskPStatus = async(statusData) => {
  const response = await api.post(`/TaskStatus`, statusData);
  return response.data;
};

const updateTaskStatus = async(id, statusData) => {
  const response = await api.put(`/TaskStatus/${id}`, statusData);
  return response.data;
};

const deleteTaskStatus = async(id) => {
  const response = await api.delete(`/TaskStatus/${id}`);
  return response.data;
};



const taskStatusService = {
  getAllTaskStatuses,
  createTaskPStatus,
  updateTaskStatus,
  deleteTaskStatus,
};

export default taskStatusService;