
function StatusBadge({ isActive }) {
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

export default StatusBadge;
