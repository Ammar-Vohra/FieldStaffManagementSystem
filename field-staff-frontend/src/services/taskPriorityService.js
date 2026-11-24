import api from "./api";

const getAllTaskPriorities = async () => {
  const response = await api.get("/TaskPriority/all");
  return response.data;
};


const createTaskPriority = async(taskData) => {
  const response = await api.post(`/TaskPriority`, taskData);
  return response.data;
};

const updateTaskPriority = async(id, taskData) => {
  const response = await api.put(`/TaskPriority/${id}`, taskData);
  return response.data;
};

const deleteTaskPriority = async(id) => {
  const response = await api.delete(`/TaskPriority/${id}`);
  return response.data;
};


const taskPriorityService = {
  getAllTaskPriorities,
  createTaskPriority,
  updateTaskPriority,
  deleteTaskPriority,
};

export default taskPriorityService;