
import { Activity, Clock, UserRound, MessageSquare } from "lucide-react";

function formatDate(date) {
    if (!date) return "N/A";

    return new Date(date).toLocaleString("en-PK", {
        day: "2-digit",
        month: "short",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
    });
}

function getStatusColor(status) {
    const value = status?.toLowerCase();

    if (value === "completed") {
        return "bg-green-100 text-green-700";
    }

    if (value === "in progress") {
        return "bg-blue-100 text-blue-700";
    }

    if (value === "pending") {
        return "bg-amber-100 text-amber-700";
    }

    return "bg-gray-100 text-gray-700";
}

function RecentActivities({ activities = [] }) {
    return (
        <div className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">

            {/* Header */}
            <div className="flex items-center gap-3 border-b border-gray-100 px-5 py-4">
                <div className="rounded-lg bg-indigo-50 p-2 text-indigo-600">
                    <Activity size={20} />
                </div>

                <div>
                    <h2 className="text-lg font-semibold text-gray-800">
                        Recent Activities
                    </h2>
                    <p className="text-sm text-gray-500">
                        Latest task status updates
                    </p>
                </div>
            </div>

            {/* Activities */}
            <div className="divide-y divide-gray-100">
                {activities.length === 0 ? (
                    <div className="px-5 py-10 text-center">
                        <Activity
                            size={32}
                            className="mx-auto mb-2 text-gray-300"
                        />

                        <p className="text-sm text-gray-500">
                            No recent activities found.
                        </p>
                    </div>
                ) : (
                    activities.map((activity) => (
                        <div
                            key={activity.taskStatusHistoryId}
                            className="px-5 py-4 transition hover:bg-gray-50"
                        >
                            {/* Task title and ID */}
                            <div className="flex flex-col gap-1 sm:flex-row sm:items-center sm:justify-between">
                                <h3 className="text-sm font-semibold text-gray-800">
                                    {activity.taskTitle}
                                </h3>

                                <span className="text-xs text-gray-400">
                                    Task #{activity.serviceTaskId}
                                </span>
                            </div>

                            {/* Status transition */}
                            <div className="mt-3 flex flex-wrap items-center gap-2">
                                {activity.oldStatusName ? (
                                    <span
                                        className={`rounded-full px-2.5 py-1 text-xs font-medium ${getStatusColor(activity.oldStatusName)}`}
                                    >
                                        {activity.oldStatusName}
                                    </span>
                                ) : (
                                    <span className="rounded-full bg-gray-100 px-2.5 py-1 text-xs font-medium text-gray-500">
                                        Created
                                    </span>
                                )}

                                <span className="text-gray-400">→</span>

                                <span
                                    className={`rounded-full px-2.5 py-1 text-xs font-medium ${getStatusColor(activity.newStatusName)}`}
                                >
                                    {activity.newStatusName}
                                </span>
                            </div>

                            {/* User and date */}
                            <div className="mt-3 flex flex-col gap-2 text-xs text-gray-500 sm:flex-row sm:items-center sm:gap-5">
                                <div className="flex items-center gap-1.5">
                                    <UserRound size={14} />
                                    <span>
                                        {activity.updatedByName || "Unknown user"}
                                    </span>
                                </div>

                                <div className="flex items-center gap-1.5">
                                    <Clock size={14} />
                                    <span>{formatDate(activity.updatedAt)}</span>
                                </div>
                            </div>

                            {/* Remarks */}
                            {activity.remarks && (
                                <div className="mt-3 flex gap-2 rounded-lg bg-gray-50 p-3 text-sm text-gray-600">
                                    <MessageSquare
                                        size={15}
                                        className="mt-0.5 shrink-0 text-gray-400"
                                    />

                                    <p className="break-words">
                                        {activity.remarks}
                                    </p>
                                </div>
                            )}
                        </div>
                    ))
                )}
            </div>
        </div>
    );
}

export default RecentActivities;
