
import api from "./api";

const getDashboardSummary = async () => {
    const response = await api.get("/Dashboard/summary");
    return response.data;
};

const dashboardService = {
    getDashboardSummary,
};

export default dashboardService;
