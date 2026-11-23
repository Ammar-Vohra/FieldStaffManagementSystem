
import {
    CalendarDays,
    MapPin,
    Building2,
    ClipboardList,
} from "lucide-react";

function FieldStaffTaskSection({ title, tasks = [], emptyMessage }) {
    const formatDate = (dateValue) => {
        if (!dateValue) return "Date not available";

        return new Date(dateValue).toLocaleString("en-PK", {
            day: "2-digit",
            month: "short",
            year: "numeric",
            hour: "2-digit",
            minute: "2-digit",
        });
    };

    const getStatusStyle = (status) => {
        switch (status?.toLowerCase()) {
            case "completed":
                return "bg-green-100 text-green-700";
            case "in progress":
                return "bg-blue-100 text-blue-700";
            case "pending":
                return "bg-yellow-100 text-yellow-700";
            default:
                return "bg-gray-100 text-gray-700";
        }
    };

    const getPriorityStyle = (priority) => {
        switch (priority?.toLowerCase()) {
            case "high":
                return "bg-red-100 text-red-700";
            case "medium":
                return "bg-orange-100 text-orange-700";
            case "low":
                return "bg-green-100 text-green-700";
            default:
                return "bg-gray-100 text-gray-700";
        }
    };

    return (
        <section className="rounded-xl border border-gray-200 bg-white p-5 shadow-sm">
            <div className="mb-4 flex items-center gap-2">
                <ClipboardList size={20} className="text-blue-600" />

                <h2 className="text-lg font-semibold text-gray-800">
                    {title}
                </h2>

                <span className="ml-auto rounded-full bg-gray-100 px-3 py-1 text-xs font-semibold text-gray-600">
                    {tasks.length}
                </span>
            </div>

            {tasks.length === 0 ? (
                <div className="flex flex-col items-center justify-center py-10 text-center">
                    <ClipboardList size={32} className="mb-2 text-gray-300" />
                    <p className="text-sm text-gray-500">
                        {emptyMessage}
                    </p>
                </div>
            ) : (
                <div className="space-y-4">
                    {tasks.map((task) => (
                        <div
                            key={task.serviceTaskId}
                            className="rounded-lg border border-gray-100 p-4 transition hover:border-blue-200 hover:bg-gray-50"
                        >
                            <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                                <div className="min-w-0">
                                    <h3 className="font-semibold text-gray-800">
                                        {task.title}
                                    </h3>

                                    <p className="mt-1 text-sm text-gray-500">
                                        {task.description || "No description provided."}
                                    </p>
                                </div>

                                <div className="flex shrink-0 flex-wrap gap-2">
                                    <span className={`rounded-full px-3 py-1 text-xs font-medium ${getStatusStyle(task.statusName)}`}>
                                        {task.statusName}
                                    </span>

                                    <span className={`rounded-full px-3 py-1 text-xs font-medium ${getPriorityStyle(task.priorityName)}`}>
                                        {task.priorityName}
                                    </span>
                                </div>
                            </div>

                            <div className="mt-4 grid grid-cols-1 gap-3 text-sm text-gray-600 sm:grid-cols-2">
                                <div className="flex items-center gap-2">
                                    <Building2 size={16} className="shrink-0 text-gray-400" />
                                    <span>{task.customerName}</span>
                                </div>

                                <div className="flex items-center gap-2">
                                    <MapPin size={16} className="shrink-0 text-gray-400" />
                                    <span>{task.serviceAddress}</span>
                                </div>

                                <div className="flex items-center gap-2 sm:col-span-2">
                                    <CalendarDays size={16} className="shrink-0 text-gray-400" />
                                    <span>{formatDate(task.scheduledDate)}</span>
                                </div>
                            </div>

                            {task.completionNote && (
                                <div className="mt-4 rounded-lg bg-green-50 p-3">
                                    <p className="text-xs font-semibold text-green-800">
                                        Completion Note
                                    </p>
                                    <p className="mt-1 text-sm text-green-700">
                                        {task.completionNote}
                                    </p>
                                </div>
                            )}
                        </div>
                    ))}
                </div>
            )}
        </section>
    );
}

export default FieldStaffTaskSection;
