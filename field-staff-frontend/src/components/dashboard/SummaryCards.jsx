
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
    const isSupervisor = summary?.overdueTasks !== undefined;

    const adminCards = [
        {
            title: "Total Tasks",
            value: summary?.totalTasks,
            description: "All active service tasks",
            icon: <ClipboardList size={22} />,
        },
        {
            title: "Pending Tasks",
            value: summary?.pendingTasks,
            description: "Tasks waiting to start",
            icon: <Clock size={22} />,
        },
        {
            title: "In Progress",
            value: summary?.inProgressTasks,
            description: "Currently ongoing tasks",
            icon: <PlayCircle size={22} />,
        },
        {
            title: "Completed Tasks",
            value: summary?.completedTasks,
            description: "Successfully completed tasks",
            icon: <CheckCircle size={22} />,
        },
        {
            title: "Field Staff",
            value: summary?.totalFieldStaff,
            description: "Active field staff members",
            icon: <Users size={22} />,
        },
        {
            title: "Total Customers",
            value: summary?.totalCustomers,
            description: "Active registered customers",
            icon: <Building2 size={22} />,
        },
    ];

    const supervisorCards = [
        {
            title: "Total Tasks",
            value: summary?.totalTasks,
            description: "All active service tasks",
            icon: <ClipboardList size={22} />,
        },
        {
            title: "Pending Tasks",
            value: summary?.pendingTasks,
            description: "Tasks waiting to start",
            icon: <Clock size={22} />,
        },
        {
            title: "In Progress",
            value: summary?.inProgressTasks,
            description: "Currently ongoing tasks",
            icon: <PlayCircle size={22} />,
        },
        {
            title: "Completed Tasks",
            value: summary?.completedTasks,
            description: "Successfully completed tasks",
            icon: <CheckCircle size={22} />,
        },
        {
            title: "Overdue Tasks",
            value: summary?.overdueTasks,
            description: "Tasks past their scheduled date",
            icon: <AlertTriangle size={22} />,
        },
        {
            title: "Field Staff",
            value: summary?.totalFieldStaff,
            description: "Active field staff members",
            icon: <UserRound size={22} />,
        },
    ];

    const cards = isSupervisor ? supervisorCards : adminCards;

    return (
        <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 xl:grid-cols-3">
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
