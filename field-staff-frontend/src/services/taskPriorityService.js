import api from "./api";

const getAllTaskPriorities = async () => {
  const response = await api.get("/TaskPriority/all");
  return response.data;
};

const taskPriorityService = {
  getAllTaskPriorities,
};

export default taskPriorityService;