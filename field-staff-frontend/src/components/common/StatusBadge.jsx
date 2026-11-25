
function StatusBadge({ isActive, status }) {
  // Record state badge (existing functionality)
  if (status === undefined) {
    return (
      <span
        className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-medium
          ${
            isActive
              ? "bg-green-100 text-green-700"
              : "bg-red-100 text-red-700"
          }
        `}
      >
        {isActive ? "Active" : "Inactive"}
      </span>
    );
  }

  // Task status badge
  const normalizedStatus = (status || "").trim().toLowerCase();

  const statusStyles = {
    pending: "bg-amber-100 text-amber-700",
    overdue: "bg-red-100 text-red-700",
    "in progress": "bg-blue-100 text-blue-700",
    completed: "bg-green-100 text-green-700",
    cancelled: "bg-slate-200 text-slate-700",
  };

  const styles =
    statusStyles[normalizedStatus] || "bg-slate-100 text-slate-700";

  return (
    <span
      className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-medium ${styles}`}
    >
      {status}
    </span>
  );
}

export default StatusBadge;
