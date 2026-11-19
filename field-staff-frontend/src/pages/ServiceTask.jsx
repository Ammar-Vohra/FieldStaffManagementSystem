import { useCallback, useEffect, useState } from "react";

import serviceTaskService from "../services/serviceTaskService";
import customerService from "../services/customerService";
import userService from "../services/userService";
import taskStatusService from "../services/taskStatusService";
import taskPriorityService from "../services/taskPriorityService";

import Modal from "../components/common/Modal";
import ConfirmDialog from "../components/common/ConfirmDialogue";
import StatusBadge from "../components/common/StatusBadge";
import DataTable from "../components/common/DataTable";
import GenericForm from "../components/common/GenericForm";
import FormInput from "../components/common/FormInput";

import SearchInput from "../components/filters/SearchInput";
import SelectFilter from "../components/filters/SelectFilter";
import SortSelect from "../components/filters/SortSelect";
import Pagination from "../components/data-display/Pagination";

// Format ISO date string for <input type="datetime-local" />
const toDateTimeLocalValue = (isoString) => {
  if (!isoString) return "";
  const date = new Date(isoString);
  if (Number.isNaN(date.getTime())) return "";
  const offsetMs = date.getTimezoneOffset() * 60000;
  return new Date(date.getTime() - offsetMs).toISOString().slice(0, 16);
};

// Format date for readable table display
const formatDisplayDate = (isoString) => {
  if (!isoString) return "—";
  const date = new Date(isoString);
  if (Number.isNaN(date.getTime())) return isoString;
  return date.toLocaleString([], {
    year: "numeric",
    month: "short",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  });
};

// Badge styling helper for priority names
const getPriorityBadgeClass = (priorityName = "") => {
  const normalized = priorityName.toLowerCase();
  if (normalized.includes("urgent") || normalized.includes("critical") || normalized.includes("high")) {
    return "bg-red-100 text-red-700";
  }
  if (normalized.includes("medium") || normalized.includes("normal")) {
    return "bg-amber-100 text-amber-700";
  }
  return "bg-slate-100 text-slate-700";
};

function ServiceTasks() {
  // Data and lookups
  const [tasks, setTasks] = useState([]);
  const [customers, setCustomers] = useState([]);
  const [users, setUsers] = useState([]);
  const [statuses, setStatuses] = useState([]);
  const [priorities, setPriorities] = useState([]);

  // Loading states
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);

  // Feedback messages
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  // Modal & dialog states
  const [showModal, setShowModal] = useState(false);
  const [showViewModal, setShowViewModal] = useState(false);
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);

  // Selected task
  const [selectedTask, setSelectedTask] = useState(null);

  // Pagination
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(0);
  const [totalRecords, setTotalRecords] = useState(0);

  // Search and filters
  const [search, setSearch] = useState("");
  const [customerFilter, setCustomerFilter] = useState("");
  const [staffFilter, setStaffFilter] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [priorityFilter, setPriorityFilter] = useState("");
  const [activeFilter, setActiveFilter] = useState("");
  const [scheduledDateFrom, setScheduledDateFrom] = useState("");
  const [scheduledDateTo, setScheduledDateTo] = useState("");

  // Sorting
  const [sortBy, setSortBy] = useState("scheduledDate");
  const [sortDirection, setSortDirection] = useState("desc");

  // Fetch paginated service tasks
  const fetchServiceTasks = useCallback(async () => {
    try {
      setLoading(true);
      setError("");

      const result = await serviceTaskService.getPaginatedServiceTasks({
        pageNumber,
        pageSize,
        search,
        sortBy,
        sortDirection,
        customerId: customerFilter ? Number(customerFilter) : undefined,
        assignedStaffId: staffFilter ? Number(staffFilter) : undefined,
        taskStatusId: statusFilter ? Number(statusFilter) : undefined,
        taskPriorityId: priorityFilter ? Number(priorityFilter) : undefined,
        scheduledDateFrom: scheduledDateFrom || undefined,
        scheduledDateTo: scheduledDateTo || undefined,
        isActive: activeFilter === "" ? undefined : activeFilter === "true",
      });

      const taskData = result.data || [];

      if (taskData.length === 0 && pageNumber > 1 && pageNumber > result.totalPages) {
        setPageNumber(result.totalPages || 1);
        return;
      }

      setTasks(taskData);
      setTotalPages(result.totalPages ?? 0);
      setTotalRecords(result.totalRecords ?? 0);
    } catch (err) {
      console.error("Failed to fetch service tasks:", err);
      setError(
        err.response?.data?.message ||
          err.response?.data?.title ||
          "Unable to load service tasks."
      );
    } finally {
      setLoading(false);
    }
  }, [
    pageNumber,
    pageSize,
    search,
    sortBy,
    sortDirection,
    customerFilter,
    staffFilter,
    statusFilter,
    priorityFilter,
    scheduledDateFrom,
    scheduledDateTo,
    activeFilter,
  ]);

  // Fetch lookup dropdowns once on mount
  useEffect(() => {
    const fetchLookups = async () => {
      try {
        const [customerData, userData, statusData, priorityData] = await Promise.all([
          customerService.getAllCustomers(),
          userService.getAllUsers(),
          taskStatusService.getAllTaskStatuses(),
          taskPriorityService.getAllTaskPriorities(),
        ]);

        setCustomers(customerData || []);
        setUsers(userData || []);
        setStatuses(statusData || []);
        setPriorities(priorityData || []);
      } catch (err) {
        console.error("Failed to load task lookup options:", err);
      }
    };

    fetchLookups();
  }, []);

  // Re-fetch tasks when query parameters change
  useEffect(() => {
    fetchServiceTasks();
  }, [fetchServiceTasks]);

  const resetSelection = () => {
    setSelectedTask(null);
  };

  const handleAdd = () => {
    resetSelection();
    setError("");
    setSuccess("");
    setShowModal(true);
  };

  const handleEdit = (task) => {
    setSelectedTask(task);
    setError("");
    setSuccess("");
    setShowModal(true);
  };

  const handleView = (task) => {
    setSelectedTask(task);
    setShowViewModal(true);
  };

  const handleCloseModal = () => {
    if (saving) return;
    setShowModal(false);
    resetSelection();
  };

  const handleResetFilters = () => {
    setSearch("");
    setCustomerFilter("");
    setStaffFilter("");
    setStatusFilter("");
    setPriorityFilter("");
    setActiveFilter("");
    setScheduledDateFrom("");
    setScheduledDateTo("");
    setSortBy("scheduledDate");
    setSortDirection("desc");
    setPageNumber(1);
  };

  // Helper to resolve IDs cleanly regardless of lookup DTO key casing
  const activeCustomerOptions = customers
    .filter((c) => c.isActive !== false)
    .map((c) => ({
      value: String(c.customerId),
      label: c.customerName,
    }));

  const activeStaffOptions = users
    .filter((u) => u.isActive !== false)
    .map((u) => ({
      value: String(u.userId),
      label: `${u.userName}${u.roleName ? ` (${u.roleName})` : ""}`,
    }));

  const statusOptions = statuses
    .filter((s) => s.isActive !== false)
    .map((s) => ({
      value: String(s.taskStatusId ?? s.statusTaskId ?? s.id),
      label: s.statusName ?? s.name ?? s.taskStatusName,
    }));

  const priorityOptions = priorities
    .filter((p) => p.isActive !== false)
    .map((p) => ({
      value: String(p.taskPriorityId ?? p.id),
      label: p.priorityName ?? p.name ?? p.taskPriorityName,
    }));

  // Form fields aligned with CreateServiceTaskRequestDto & UpdateServiceTaskRequestDto
  const taskFields = [
    {
      name: "title",
      label: "Task Title",
      type: "text",
      placeholder: "Enter task title",
      required: true,
      minLength: 3,
      maxLength: 200,
    },
    {
      name: "description",
      label: "Description",
      type: "text",
      placeholder: "Enter task instructions or details (optional)",
      maxLength: 1000,
    },
    {
      name: "customerId",
      label: "Customer",
      type: "select",
      placeholder: "Select a customer",
      required: true,
      options: activeCustomerOptions,
    },
    // AssignedStaffId & TaskStatusId are set on Create; later managed via dedicated endpoints
    ...(!selectedTask
      ? [
          {
            name: "assignedStaffId",
            label: "Assigned Field Staff",
            type: "select",
            placeholder: "Select field staff",
            required: true,
            options: activeStaffOptions,
          },
          {
            name: "taskStatusId",
            label: "Initial Status",
            type: "select",
            placeholder: "Select initial status",
            required: true,
            options: statusOptions,
          },
        ]
      : []),
    {
      name: "taskPriorityId",
      label: "Priority",
      type: "select",
      placeholder: "Select priority",
      required: true,
      options: priorityOptions,
    },
    {
      name: "scheduledDate",
      label: "Scheduled Date & Time",
      type: "datetime-local",
      required: true,
    },
    // CompletionNote is supported by UpdateServiceTaskRequestDto
    ...(selectedTask
      ? [
          {
            name: "completionNote",
            label: "Completion Note",
            type: "text",
            placeholder: "Add completion remarks (optional)",
            maxLength: 1000,
          },
        ]
      : []),
  ];

  const initialFormData = selectedTask
    ? {
        title: selectedTask.title ?? "",
        description: selectedTask.description ?? "",
        customerId: String(selectedTask.customerId ?? ""),
        taskPriorityId: String(selectedTask.taskPriorityId ?? ""),
        scheduledDate: toDateTimeLocalValue(selectedTask.scheduledDate),
        completionNote: selectedTask.completionNote ?? "",
      }
    : {
        title: "",
        description: "",
        customerId: "",
        assignedStaffId: "",
        taskStatusId: "",
        taskPriorityId: "",
        scheduledDate: "",
      };

  // Create or Update Service Task
  const handleSubmit = async (formData, setFormErrors) => {
    setError("");
    setSuccess("");
    setFormErrors({});

    const scheduledIso = new Date(formData.scheduledDate).toISOString();

    try {
      setSaving(true);

      if (selectedTask) {
        const updatePayload = {
          title: formData.title.trim(),
          description: formData.description?.trim() || null,
          customerId: Number(formData.customerId),
          taskPriorityId: Number(formData.taskPriorityId),
          scheduledDate: scheduledIso,
          completionNote: formData.completionNote?.trim() || null,
        };

        await serviceTaskService.updateServiceTask(
          selectedTask.serviceTaskId,
          updatePayload
        );
        setSuccess("Service task updated successfully.");
      } else {
        const createPayload = {
          title: formData.title.trim(),
          description: formData.description?.trim() || null,
          customerId: Number(formData.customerId),
          assignedStaffId: Number(formData.assignedStaffId),
          taskStatusId: Number(formData.taskStatusId),
          taskPriorityId: Number(formData.taskPriorityId),
          scheduledDate: scheduledIso,
        };

        await serviceTaskService.createServiceTask(createPayload);
        setSuccess("Service task created successfully.");
      }

      setShowModal(false);
      resetSelection();
      await fetchServiceTasks();
    } catch (err) {
      console.error("Failed to save service task:", err);
      setFormErrors({
        general:
          err.response?.data?.message ||
          err.response?.data?.title ||
          "Unable to save service task.",
      });
    } finally {
      setSaving(false);
    }
  };

  // Delete confirmation & execution
  const handleDeleteClick = (task) => {
    setSelectedTask(task);
    setShowDeleteDialog(true);
    setError("");
    setSuccess("");
  };

  const handleDelete = async () => {
    if (!selectedTask) return;

    try {
      setDeleting(true);
      setError("");
      setSuccess("");

      await serviceTaskService.deleteServiceTask(selectedTask.serviceTaskId);
      setSuccess("Service task deleted successfully.");
      setShowDeleteDialog(false);

      if (tasks.length === 1 && pageNumber > 1) {
        setPageNumber((prev) => prev - 1);
      } else {
        await fetchServiceTasks();
      }

      setSelectedTask(null);
    } catch (err) {
      console.error("Failed to delete service task:", err);
      setError(
        err.response?.data?.message ||
          err.response?.data?.title ||
          "Unable to delete service task."
      );
    } finally {
      setDeleting(false);
    }
  };

  // DataTable Columns
  const columns = [
    {
      key: "serviceTaskId",
      label: "ID",
    },
    {
      key: "title",
      label: "Title",
      render: (task) => (
        <div className="max-w-xs truncate font-medium text-slate-800" title={task.title}>
          {task.title}
        </div>
      ),
    },
    {
      key: "customerName",
      label: "Customer",
    },
    {
      key: "assignedStaffName",
      label: "Assigned Staff",
    },
    {
      key: "taskStatusName",
      label: "Task Status",
      render: (task) => (
        <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-700">
          {task.taskStatusName}
        </span>
      ),
    },
    {
      key: "taskPriorityName",
      label: "Priority",
      render: (task) => (
        <span
          className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-medium ${getPriorityBadgeClass(
            task.taskPriorityName
          )}`}
        >
          {task.taskPriorityName}
        </span>
      ),
    },
    {
      key: "scheduledDate",
      label: "Scheduled Date",
      render: (task) => formatDisplayDate(task.scheduledDate),
    },
    {
      key: "isActive",
      label: "Record State",
      render: (task) => <StatusBadge isActive={task.isActive} />,
    },
    {
      key: "actions",
      label: "Actions",
      render: (task) => (
        <div className="flex items-center gap-2">
          <button
            onClick={() => handleView(task)}
            className="px-3 py-1.5 text-sm font-medium text-slate-700 bg-slate-100 hover:bg-slate-200 rounded-lg transition"
          >
            View
          </button>

          <button
            onClick={() => handleEdit(task)}
            className="px-3 py-1.5 text-sm font-medium text-blue-700 bg-blue-100 hover:bg-blue-200 rounded-lg transition"
          >
            Edit
          </button>

          <button
            onClick={() => handleDeleteClick(task)}
            className="px-3 py-1.5 text-sm font-medium text-red-700 bg-red-100 hover:bg-red-200 rounded-lg transition"
          >
            Delete
          </button>
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-slate-800">
            Service Task Management
          </h1>
          <p className="text-slate-500 mt-1">
            Schedule, assign, and track field service tasks.
          </p>
        </div>

        <button
          onClick={handleAdd}
          className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-medium transition"
        >
          + Create Service Task
        </button>
      </div>

      {/* Feedback Banners */}
      {success && (
        <div className="bg-green-100 text-green-700 px-4 py-3 rounded-lg">
          {success}
        </div>
      )}

      {error && (
        <div className="bg-red-100 text-red-700 px-4 py-3 rounded-lg">
          {error}
        </div>
      )}

      {/* Search, Filters & Sorting */}
      <div className="bg-white p-5 rounded-xl border border-slate-200 space-y-4">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <SearchInput
            label="Search Tasks"
            value={search}
            onChange={(e) => {
              setSearch(e.target.value);
              setPageNumber(1);
            }}
            placeholder="Search title, customer, or staff..."
          />

          <SelectFilter
            label="Customer"
            value={customerFilter}
            onChange={(e) => {
              setCustomerFilter(e.target.value);
              setPageNumber(1);
            }}
            placeholder="All Customers"
            options={customers.map((c) => ({
              value: String(c.customerId),
              label: c.customerName,
            }))}
          />

          <SelectFilter
            label="Assigned Staff"
            value={staffFilter}
            onChange={(e) => {
              setStaffFilter(e.target.value);
              setPageNumber(1);
            }}
            placeholder="All Staff"
            options={users.map((u) => ({
              value: String(u.userId),
              label: u.userName,
            }))}
          />

          <SelectFilter
            label="Task Status"
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value);
              setPageNumber(1);
            }}
            placeholder="All Statuses"
            options={statusOptions}
          />

          <SelectFilter
            label="Priority"
            value={priorityFilter}
            onChange={(e) => {
              setPriorityFilter(e.target.value);
              setPageNumber(1);
            }}
            placeholder="All Priorities"
            options={priorityOptions}
          />

          <FormInput
            label="Scheduled From"
            name="scheduledDateFrom"
            type="date"
            value={scheduledDateFrom}
            onChange={(e) => {
              setScheduledDateFrom(e.target.value);
              setPageNumber(1);
            }}
          />

          <FormInput
            label="Scheduled To"
            name="scheduledDateTo"
            type="date"
            value={scheduledDateTo}
            onChange={(e) => {
              setScheduledDateTo(e.target.value);
              setPageNumber(1);
            }}
          />

          <SelectFilter
            label="Record State"
            value={activeFilter}
            onChange={(e) => {
              setActiveFilter(e.target.value);
              setPageNumber(1);
            }}
            placeholder="All Records"
            options={[
              { value: "true", label: "Active" },
              { value: "false", label: "Inactive" },
            ]}
          />
        </div>

        <div className="flex flex-col sm:flex-row sm:items-end justify-between gap-4 pt-2 border-t border-slate-100">
          <div className="w-full sm:w-72">
            <SortSelect
              label="Sort By"
              value={`${sortBy}|${sortDirection}`}
              onChange={(e) => {
                const [column, direction] = e.target.value.split("|");
                setSortBy(column);
                setSortDirection(direction);
                setPageNumber(1);
              }}
              options={[
                { value: "scheduledDate|desc", label: "Scheduled Date (Latest First)" },
                { value: "scheduledDate|asc", label: "Scheduled Date (Earliest First)" },
                { value: "createdAt|desc", label: "Created Date (Newest First)" },
                { value: "createdAt|asc", label: "Created Date (Oldest First)" },
                { value: "title|asc", label: "Title (A-Z)" },
                { value: "title|desc", label: "Title (Z-A)" },
                { value: "customerName|asc", label: "Customer (A-Z)" },
                { value: "assignedStaffName|asc", label: "Assigned Staff (A-Z)" },
              ]}
            />
          </div>

          <button
            type="button"
            onClick={handleResetFilters}
            className="px-4 py-2 text-sm font-medium text-slate-600 border border-slate-300 rounded-lg hover:bg-slate-50 transition self-start sm:self-end"
          >
            Reset Filters
          </button>
        </div>
      </div>

      {/* Service Tasks Table */}
      <DataTable
        columns={columns}
        data={tasks}
        loading={loading}
        emptyMessage="No service tasks found."
        getRowKey={(task) => task.serviceTaskId}
      />

      {/* Pagination */}
      <Pagination
        pageNumber={pageNumber}
        pageSize={pageSize}
        totalPages={totalPages}
        totalRecords={totalRecords}
        onPageChange={setPageNumber}
        onPageSizeChange={(size) => {
          setPageSize(size);
          setPageNumber(1);
        }}
        loading={loading}
      />

      {/* Create / Edit Service Task Modal */}
      <Modal
        isOpen={showModal}
        onClose={handleCloseModal}
        title={selectedTask ? "Edit Service Task" : "Create Service Task"}
        size="lg"
      >
        <GenericForm
          key={selectedTask?.serviceTaskId ?? "new-service-task"}
          fields={taskFields}
          initialData={initialFormData}
          saving={saving}
          submitText={selectedTask ? "Update Task" : "Create Task"}
          onSubmit={handleSubmit}
          onCancel={handleCloseModal}
        />
      </Modal>

      {/* Task Details View Modal */}
      <Modal
        isOpen={showViewModal}
        onClose={() => {
          setShowViewModal(false);
          resetSelection();
        }}
        title={`Service Task #${selectedTask?.serviceTaskId ?? ""}`}
        size="lg"
      >
        {selectedTask && (
          <div className="space-y-4 text-sm">
            <div>
              <p className="text-slate-500">Title</p>
              <p className="font-semibold text-slate-800 text-base">
                {selectedTask.title}
              </p>
            </div>

            <div>
              <p className="text-slate-500">Description</p>
              <p className="text-slate-700 whitespace-pre-wrap">
                {selectedTask.description || "No description provided."}
              </p>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 pt-2 border-t border-slate-100">
              <div>
                <p className="text-slate-500">Customer</p>
                <p className="font-medium text-slate-800">
                  {selectedTask.customerName}
                </p>
              </div>

              <div>
                <p className="text-slate-500">Assigned Staff</p>
                <p className="font-medium text-slate-800">
                  {selectedTask.assignedStaffName}
                </p>
              </div>

              <div>
                <p className="text-slate-500">Status</p>
                <p className="font-medium text-slate-800">
                  {selectedTask.taskStatusName}
                </p>
              </div>

              <div>
                <p className="text-slate-500">Priority</p>
                <p className="font-medium text-slate-800">
                  {selectedTask.taskPriorityName}
                </p>
              </div>

              <div>
                <p className="text-slate-500">Scheduled Date</p>
                <p className="font-medium text-slate-800">
                  {formatDisplayDate(selectedTask.scheduledDate)}
                </p>
              </div>

              <div>
                <p className="text-slate-500">Created At</p>
                <p className="font-medium text-slate-800">
                  {formatDisplayDate(selectedTask.createdAt)}
                </p>
              </div>
            </div>

            {selectedTask.completionNote && (
              <div className="pt-2 border-t border-slate-100">
                <p className="text-slate-500">Completion Note</p>
                <p className="text-slate-700 whitespace-pre-wrap">
                  {selectedTask.completionNote}
                </p>
              </div>
            )}

            <div className="flex justify-end pt-4 border-t border-slate-100">
              <button
                type="button"
                onClick={() => {
                  setShowViewModal(false);
                  resetSelection();
                }}
                className="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50"
              >
                Close
              </button>
            </div>
          </div>
        )}
      </Modal>

      {/* Delete Confirmation Dialog */}
      <ConfirmDialog
        isOpen={showDeleteDialog}
        onClose={() => {
          if (!deleting) {
            setShowDeleteDialog(false);
            setSelectedTask(null);
          }
        }}
        onConfirm={handleDelete}
        title="Delete Service Task"
        message={`Are you sure you want to delete "${selectedTask?.title}"?`}
        confirmText="Delete"
        loading={deleting}
      />
    </div>
  );
}

export default ServiceTasks;