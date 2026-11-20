
import { useCallback, useEffect, useState } from "react";

import dashboardService from "../services/dashboardService";

import SummaryCards from "../components/dashboard/SummaryCards";
import TaskStatusChart from "../components/dashboard/TaskStatusChart";
import TodaysTasks from "../components/dashboard/TodaysTasks";
import RecentTasks from "../components/dashboard/RecentTasks";
import StaffWorkload from "../components/dashboard/StaffWorkload";

function Dashboard() {
    const [summary, setSummary] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const fetchDashboardSummary = useCallback(async () => {
        try {
            setLoading(true);
            setError("");

            const data = await dashboardService.getDashboardSummary();

            setSummary(data);
        } catch (err) {
            console.error("Dashboard loading error:", err);

            setError(
                err.response?.data?.message ||
                "Unable to load dashboard data. Please try again."
            );
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        fetchDashboardSummary();
    }, [fetchDashboardSummary]);

    if (loading) {
        return (
            <div className="flex min-h-[60vh] items-center justify-center">
                <div className="flex flex-col items-center gap-3">
                    <div className="h-10 w-10 animate-spin rounded-full border-4 border-blue-600 border-t-transparent"></div>
                    <p className="text-sm font-medium text-gray-500">
                        Loading dashboard...
                    </p>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="flex min-h-[50vh] flex-col items-center justify-center gap-4">
                <p className="text-center text-sm text-red-600">
                    {error}
                </p>

                <button
                    onClick={fetchDashboardSummary}
                    className="rounded-lg bg-blue-600 px-5 py-2 text-sm font-medium text-white transition hover:bg-blue-700"
                >
                    Retry
                </button>
            </div>
        );
    }

    if (!summary) {
        return null;
    }

    const isSupervisor = summary.overdueTasks !== undefined;

    return (
        <div className="space-y-6 p-4 md:p-6">

            {/* Dashboard Header */}
            <div>
                <h1 className="text-2xl font-bold text-gray-800">
                    {isSupervisor ? "Supervisor Dashboard" : "Admin Dashboard"}
                </h1>

                <p className="mt-1 text-sm text-gray-500">
                    {isSupervisor
                        ? "Monitor service tasks and field staff performance."
                        : "Overview of your field staff operations and service tasks."}
                </p>
            </div>

            {/* Summary Cards */}
            <SummaryCards summary={summary} />

            {/* Task Status and Staff Workload */}
            <div className="grid grid-cols-1 gap-6 xl:grid-cols-2">

                <TaskStatusChart
                    data={summary.taskStatusDistribution || []}
                />

                <StaffWorkload
                    staff={summary.staffWorkload || []}
                />

            </div>

            {/* Today's Tasks */}
            <TodaysTasks
                tasks={summary.todaysTasks || []}
            />

            {/* Recent Tasks */}
            <RecentTasks
                tasks={summary.recentTasks || []}
            />

        </div>
    );
}

export default Dashboard;
