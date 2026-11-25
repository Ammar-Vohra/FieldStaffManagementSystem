import StatusBadge from "../common/StatusBadge";

function RecentTasks({ tasks = [] }) {
    const formatDate = (date) => {
        if (!date) return "-";

        return new Date(date).toLocaleDateString();
    };

    return (
        <div className="rounded-xl border border-gray-200 bg-white p-5 shadow-sm">
            <div className="mb-5 flex items-center justify-between">
                <h3 className="text-base font-semibold text-gray-800">
                    Recent Tasks
                </h3>

                <span className="text-sm text-gray-500">
                    Latest 5
                </span>
            </div>

            {tasks.length === 0 ? (
                <p className="py-8 text-center text-sm text-gray-400">
                    No recent tasks available.
                </p>
            ) : (
                <div className="overflow-x-auto">
                    <table className="w-full whitespace-nowrap text-left text-sm">
                        <thead className="bg-gray-50 text-xs uppercase text-gray-500">
                            <tr>
                                <th className="px-4 py-3">Task</th>
                                <th className="px-4 py-3">Customer</th>
                                <th className="px-4 py-3">Assigned Staff</th>
                                <th className="px-4 py-3">Status</th>
                                <th className="px-4 py-3">Created At</th>
                            </tr>
                        </thead>

                        <tbody className="divide-y divide-gray-100">
                            {tasks.map((task) => (
                                <tr
                                    key={task.serviceTaskId}
                                    className="hover:bg-gray-50"
                                >
                                    <td className="px-4 py-3 font-medium text-gray-800">
                                        {task.title}
                                    </td>

                                    <td className="px-4 py-3 text-gray-600">
                                        {task.customerName}
                                    </td>

                                    <td className="px-4 py-3 text-gray-600">
                                        {task.assignedStaffName}
                                    </td>

                                    <td className="px-4 py-3">
                                        <StatusBadge status={task.statusName} />
                                    </td>

                                    <td className="px-4 py-3 text-gray-600">
                                        {formatDate(task.createdAt)}
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
}

export default RecentTasks;