
import {
    ClipboardList,
    Clock,
    PlayCircle,
    CheckCircle,
    Users,
    UserRound,
    AlertTriangle,
    Building2,
} from "lucide-react";

import SummaryCard from "./SummaryCard";

function SummaryCards({ summary }) {
    const isFieldStaff = Array.isArray(summary?.upcomingTasks);
    const isSupervisor = summary?.overdueTasks !== undefined;

    const adminCards = [
        {
            title: "Total Tasks",
            value: summary?.totalTasks ?? 0,
            description: "All active service tasks",
            icon: <ClipboardList size={22} />,
        },
        {
            title: "Pending Tasks",
            value: summary?.pendingTasks ?? 0,
            description: "Tasks waiting to start",
            icon: <Clock size={22} />,
        },
        {
            title: "In Progress",
            value: summary?.inProgressTasks ?? 0,
            description: "Currently ongoing tasks",
            icon: <PlayCircle size={22} />,
        },
        {
            title: "Completed Tasks",
            value: summary?.completedTasks ?? 0,
            description: "Successfully completed tasks",
            icon: <CheckCircle size={22} />,
        },
        {
            title: "Field Staff",
            value: summary?.totalFieldStaff ?? 0,
            description: "Active field staff members",
            icon: <Users size={22} />,
        },
        {
            title: "Total Customers",
            value: summary?.totalCustomers ?? 0,
            description: "Active registered customers",
            icon: <Building2 size={22} />,
        },
    ];

    const supervisorCards = [
        {
            title: "Total Tasks",
            value: summary?.totalTasks ?? 0,
            description: "All active service tasks",
            icon: <ClipboardList size={22} />,
        },
        {
            title: "Pending Tasks",
            value: summary?.pendingTasks ?? 0,
            description: "Tasks waiting to start",
            icon: <Clock size={22} />,
        },
        {
            title: "In Progress",
            value: summary?.inProgressTasks ?? 0,
            description: "Currently ongoing tasks",
            icon: <PlayCircle size={22} />,
        },
        {
            title: "Completed Tasks",
            value: summary?.completedTasks ?? 0,
            description: "Successfully completed tasks",
            icon: <CheckCircle size={22} />,
        },
        {
            title: "Overdue Tasks",
            value: summary?.overdueTasks ?? 0,
            description: "Tasks past their scheduled date",
            icon: <AlertTriangle size={22} />,
        },
        {
            title: "Field Staff",
            value: summary?.totalFieldStaff ?? 0,
            description: "Active field staff members",
            icon: <UserRound size={22} />,
        },
    ];

    const fieldStaffCards = [
        {
            title: "My Tasks",
            value: summary?.totalTasks ?? 0,
            description: "All tasks assigned to you",
            icon: <ClipboardList size={22} />,
        },
        {
            title: "Pending Tasks",
            value: summary?.pendingTasks ?? 0,
            description: "Tasks waiting to start",
            icon: <Clock size={22} />,
        },
        {
            title: "In Progress",
            value: summary?.inProgressTasks ?? 0,
            description: "Tasks currently in progress",
            icon: <PlayCircle size={22} />,
        },
        {
            title: "Completed Tasks",
            value: summary?.completedTasks ?? 0,
            description: "Tasks you have completed",
            icon: <CheckCircle size={22} />,
        },
    ];

    const cards = isFieldStaff
        ? fieldStaffCards
        : isSupervisor
            ? supervisorCards
            : adminCards;

    return (
        <div className={`grid grid-cols-1 gap-5 sm:grid-cols-2 ${
            isFieldStaff ? "xl:grid-cols-4" : "xl:grid-cols-3"
        }`}>
            {cards.map((card) => (
                <SummaryCard
                    key={card.title}
                    {...card}
                />
            ))}
        </div>
    );
}

export default SummaryCards;
