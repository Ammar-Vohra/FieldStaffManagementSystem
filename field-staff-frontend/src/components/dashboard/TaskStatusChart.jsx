function TaskStatusChart({ data = [] }) {
    const total = data.reduce((sum, item) => sum + item.count, 0);

    return (
        <div className="rounded-xl border border-gray-200 bg-white p-5 shadow-sm">
            <div className="mb-6 flex items-center justify-between">
                <h3 className="text-base font-semibold text-gray-800">
                    Task Status Distribution
                </h3>

                <span className="text-sm text-gray-500">
                    {total} Tasks
                </span>
            </div>

            {data.length === 0 ? (
                <p className="py-8 text-center text-sm text-gray-400">
                    No task status data available.
                </p>
            ) : (
                <div className="space-y-5">
                    {data.map((item) => {
                        const percentage = total > 0
                            ? (item.count / total) * 100
                            : 0;

                        return (
                            <div key={item.statusName}>
                                <div className="mb-2 flex justify-between text-sm">
                                    <span className="text-gray-600">
                                        {item.statusName}
                                    </span>

                                    <span className="font-semibold text-gray-800">
                                        {item.count}
                                    </span>
                                </div>

                                <div className="h-2 w-full overflow-hidden rounded-full bg-gray-100">
                                    <div
                                        className="h-full rounded-full bg-blue-600 transition-all duration-300"
                                        style={{ width: `${percentage}%` }}
                                    />
                                </div>
                            </div>
                        );
                    })}
                </div>
            )}
        </div>
    );
}

export default TaskStatusChart;