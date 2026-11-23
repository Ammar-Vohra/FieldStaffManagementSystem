
import api from "./api";

const getMyPermissions = async () => {
  const response = await api.get("/UserPermissions/my-permissions");
  return response.data;
};

const userPermissionService = {
  getMyPermissions,
};

export default userPermissionService;
