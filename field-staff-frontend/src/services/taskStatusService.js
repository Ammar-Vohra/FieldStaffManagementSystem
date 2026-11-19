import api from "./api";

const getAllTaskStatuses = async () => {
  const response = await api.get("/TaskStatus/all");
  return response.data;
};

const taskStatusService = {
  getAllTaskStatuses,
};

export default taskStatusService;