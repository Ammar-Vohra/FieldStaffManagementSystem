import api from "./api";

const login  = async (credentials) => {
    const response = await api.post("/Auth/login", credentials);
    return response.data;
  };

const authService = {
    login,
};

export default authService;