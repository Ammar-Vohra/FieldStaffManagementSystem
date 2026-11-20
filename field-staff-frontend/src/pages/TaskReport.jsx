
import { useAuth } from "../context/AuthContext";
import { useOutletContext } from "react-router-dom";

function TaskReport() {
  const { user } = useAuth();
  const { roleName } = useOutletContext();

  return (
    <div className="p-8">
      <h1 className="text-3xl font-bold text-slate-800">
        Welcome, {user?.userName}!
      </h1>

      <p className="mt-3 text-slate-600">
        You have successfully logged in to the Field Staff
        Tracking & Management System.
      </p>
    </div>
  );
}

export default TaskReport;
