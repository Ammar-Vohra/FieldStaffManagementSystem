function StaffWorkload({ staff = [] }) {
    return (
        <div className="rounded-xl border border-gray-200 bg-white p-5 shadow-sm">
            <div className="mb-5 flex items-center justify-between">
                <h3 className="text-base font-semibold text-gray-800">
                    Staff Workload
                </h3>

                <span className="text-sm text-gray-500">
                    {staff.length} Staff Members
                </span>
            </div>

            {staff.length === 0 ? (
                <p className="py-8 text-center text-sm text-gray-400">
                    No field staff workload available.
                </p>
            ) : (
                <div className="overflow-x-auto">
                    <table className="w-full whitespace-nowrap text-left text-sm">
                        <thead className="bg-gray-50 text-xs uppercase text-gray-500">
                            <tr>
                                <th className="px-4 py-3">Staff Member</th>
                                <th className="px-4 py-3">Total Tasks</th>
                                <th className="px-4 py-3">Pending</th>
                                <th className="px-4 py-3">In Progress</th>
                                {/* <th className="px-4 py-3">Overdue</th> */}
                                <th className="px-4 py-3">Completed</th>
                            </tr>
                        </thead>

                        <tbody className="divide-y divide-gray-100">
                            {staff.map((member) => (
                                <tr
                                    key={member.userId}
                                    className="hover:bg-gray-50"
                                >
                                    <td className="px-4 py-3 font-medium text-gray-800">
                                        {member.staffName}
                                    </td>

                                    <td className="px-4 py-3 text-gray-600">
                                        {member.totalAssignedTasks}
                                    </td>

                                    <td className="px-4 py-3 text-gray-600">
                                        {member.pendingTasks}
                                    </td>

                                    <td className="px-4 py-3 text-gray-600">
                                        {member.inProgressTasks}
                                    </td>

                                    {/* <td className="px-4 py-3 text-gray-600">
                                        {member.overDueTasks}
                                    </td> */}

                                    <td className="px-4 py-3 text-gray-600">
                                        {member.completedTasks}
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

export default StaffWorkload;