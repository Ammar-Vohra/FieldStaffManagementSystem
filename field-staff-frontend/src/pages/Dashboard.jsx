
import { useCallback, useEffect, useState } from "react";

import dashboardService from "../services/dashboardService";

import SummaryCards from "../components/dashboard/SummaryCards";
import TaskStatusChart from "../components/dashboard/TaskStatusChart";
import TodaysTasks from "../components/dashboard/TodaysTasks";
import RecentTasks from "../components/dashboard/RecentTasks";
import StaffWorkload from "../components/dashboard/StaffWorkload";
import RecentActivities from "../components/dashboard/RecentActivities";
import FieldStaffTaskSection from "../components/dashboard/FieldStaffTaskSection";

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

    // Identify dashboard type from the API response structure.
    const isFieldStaff = Array.isArray(summary.upcomingTasks);
    const isSupervisor = summary.overdueTasks !== undefined;

    const dashboardTitle = isFieldStaff
        ? "Field Staff Dashboard"
        : isSupervisor
            ? "Supervisor Dashboard"
            : "Admin Dashboard";

    const dashboardDescription = isFieldStaff
        ? "View your assigned tasks and track your daily work."
        : isSupervisor
            ? "Monitor service tasks and field staff performance."
            : "Overview of your field staff operations and service tasks.";

    return (
        <div className="space-y-6 p-4 md:p-6">

            {/* Dashboard Header */}
            <div>
                <h1 className="text-2xl font-bold text-gray-800">
                    {dashboardTitle}
                </h1>

                <p className="mt-1 text-sm text-gray-500">
                    {dashboardDescription}
                </p>
            </div>

            {/* Summary Cards */}
            <SummaryCards summary={summary} />

            {/* Field Staff Dashboard */}
            {isFieldStaff ? (
                <div className="space-y-6">

                    <FieldStaffTaskSection
                        title="Today's Tasks"
                        tasks={summary.todaysTasks || []}
                        emptyMessage="You have no tasks scheduled for today."
                    />

                    <FieldStaffTaskSection
                        title="Upcoming Tasks"
                        tasks={summary.upcomingTasks || []}
                        emptyMessage="You have no upcoming tasks."
                    />

                    <FieldStaffTaskSection
                        title="Recently Completed Tasks"
                        tasks={summary.recentCompletedTasks || []}
                        emptyMessage="You haven't completed any tasks yet."
                    />

                </div>
            ) : (
                <>
                    {/* Admin and Supervisor Dashboard */}
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

                    {/* Recent Activities - Supervisor only */}
                    {isSupervisor && (
                        <RecentActivities
                            activities={summary.recentActivities || []}
                        />
                    )}
                </>
            )}

        </div>
    );
}

export default Dashboard;
