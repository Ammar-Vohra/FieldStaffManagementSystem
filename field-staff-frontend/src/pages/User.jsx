
import { useCallback, useEffect, useState } from "react";

import userService from "../services/userService";
import roleService from "../services/roleService";

import Modal from "../components/common/Modal";
import ConfirmDialog from "../components/common/ConfirmDialogue";
import StatusBadge from "../components/common/StatusBadge";
import DataTable from "../components/common/DataTable";
import GenericForm from "../components/common/GenericForm";

import SearchInput from "../components/filters/SearchInput";
import SelectFilter from "../components/filters/SelectFilter";
import SortSelect from "../components/filters/SortSelect";
import Pagination from "../components/data-display/Pagination";

function Users() {

    // Users and roles
    const [users, setUsers] = useState([]);
    const [roles, setRoles] = useState([]);

    // Loading states
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);

    // Messages
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    // Modal states
    const [showModal, setShowModal] = useState(false);
    const [showDeleteDialog, setShowDeleteDialog] = useState(false);

    // Selected user
    const [selectedUser, setSelectedUser] = useState(null);

    // Pagination
    const [pageNumber, setPageNumber] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(0);
    const [totalRecords, setTotalRecords] = useState(0);
    const [hasPreviousPage, setHasPreviousPage] = useState(false);
    const [hasNextPage, setHasNextPage] = useState(false);

    // Search and filters
    const [search, setSearch] = useState("");
    const [roleFilter, setRoleFilter] = useState("");
    const [statusFilter, setStatusFilter] = useState("");

    // Sorting
    const [sortBy, setSortBy] = useState("userName");
    const [sortDirection, setSortDirection] = useState("asc");


    // Fetch paginated users
    const fetchUsers = useCallback(async () => {
        try {
            setLoading(true);
            setError("");

            const result = await userService.getPaginatedUsers({
                pageNumber,
                pageSize,
                search,
                sortBy,
                sortDirection,
                roleId: roleFilter || undefined,
                isActive: statusFilter === ""
                    ? undefined
                    : statusFilter === "true"
            });

            const userData = result.data || [];

            // Handle deletion of last record on a page
            if (
                userData.length === 0 &&
                pageNumber > 1 &&
                pageNumber > result.totalPages
            ) {
                setPageNumber(result.totalPages || 1);
                return;
            }

            setUsers(userData);
            setTotalPages(result.totalPages);
            setTotalRecords(result.totalRecords);
            setHasPreviousPage(result.hasPreviousPage);
            setHasNextPage(result.hasNextPage);

        } catch (err) {
            console.error("Failed to fetch users:", err);

            setError(
                err.response?.data?.message ||
                err.response?.data?.title ||
                "Unable to load users."
            );

        } finally {
            setLoading(false);
        }
    }, [
        pageNumber,
        pageSize,
        search,
        roleFilter,
        statusFilter,
        sortBy,
        sortDirection
    ]);


    // Fetch roles for dropdown
    useEffect(() => {
        const fetchRoles = async () => {
            try {
                const data = await roleService.getAllRoles();
                setRoles(data);
            } catch (err) {
                console.error("Failed to fetch roles:", err);
            }
        };

        fetchRoles();
    }, []);


    // Fetch users whenever filters or pagination change
    useEffect(() => {
        fetchUsers();
    }, [fetchUsers]);


    // Reset selected user
    const resetForm = () => {
        setSelectedUser(null);
    };


    // Open Add User Modal
    const handleAdd = () => {
        resetForm();
        setError("");
        setSuccess("");
        setShowModal(true);
    };


    // Open Edit User Modal
    const handleEdit = (user) => {
        setSelectedUser(user);
        setError("");
        setSuccess("");
        setShowModal(true);
    };


    // Close Modal
    const handleCloseModal = () => {
        if (saving) return;

        setShowModal(false);
        resetForm();
    };


    // User form fields
    const userFields = [
        {
            name: "userName",
            label: "Username",
            type: "text",
            placeholder: "Enter username",
            required: true,
            minLength: 3,
            maxLength: 100
        },
        {
            name: "email",
            label: "Email",
            type: "email",
            placeholder: "Enter email address",
            required: true,
            maxLength: 30
        },

        // Password is only required when creating a user
        ...(!selectedUser ? [{
            name: "password",
            label: "Password",
            type: "password",
            placeholder: "Enter password (minimum 8 characters)",
            required: true,
            minLength: 8,
            maxLength: 100
        }] : []),

        {
            name: "roleId",
            label: "Role",
            type: "select",
            placeholder: "Select a role",
            required: true,
            options: roles
                .filter(role => role.isActive)
                .map(role => ({
                    value: role.id,
                    label: role.roleName
                }))
        },
        {
            name: "isActive",
            label: "Active",
            type: "checkbox"
        }
    ];


    // Initial form values
    const initialFormData = selectedUser
        ? {
            userName: selectedUser.userName,
            email: selectedUser.email,
            roleId: String(selectedUser.roleId),
            isActive: selectedUser.isActive
        }
        : {
            userName: "",
            email: "",
            password: "",
            roleId: "",
            isActive: true
        };


    // Save User (Create / Update)
    const handleSubmit = async (formData, setFormErrors) => {

        setError("");
        setSuccess("");
        setFormErrors({});

        const userData = {
            userName: formData.userName.trim(),
            email: formData.email.trim(),
            roleId: Number(formData.roleId),
            isActive: formData.isActive ?? true
        };

        try {
            setSaving(true);

            if (selectedUser) {

                await userService.updateUser(
                    selectedUser.userId,
                    userData
                );

                setSuccess("User updated successfully.");

            } else {

                await userService.createUser({
                    ...userData,
                    password: formData.password
                });

                setSuccess("User created successfully.");
            }

            setShowModal(false);
            resetForm();

            await fetchUsers();

        } catch (err) {
            console.error("Failed to save user:", err);

            setFormErrors({
                general:
                    err.response?.data?.message ||
                    err.response?.data?.title ||
                    "Unable to save user."
            });

        } finally {
            setSaving(false);
        }
    };


    // Open Delete Confirmation
    const handleDeleteClick = (user) => {
        setSelectedUser(user);
        setShowDeleteDialog(true);
        setError("");
        setSuccess("");
    };


    // Delete User
    const handleDelete = async () => {
        if (!selectedUser) return;

        try {
            setDeleting(true);
            setError("");
            setSuccess("");

            await userService.deleteUser(selectedUser.userId);

            setSuccess("User deleted successfully.");
            setShowDeleteDialog(false);

            // If deleting the last record on the current page,
            // move to the previous page.
            if (users.length === 1 && pageNumber > 1) {
                setPageNumber(prev => prev - 1);
            } else {
                await fetchUsers();
            }

            setSelectedUser(null);

        } catch (err) {
            console.error("Failed to delete user:", err);

            setError(
                err.response?.data?.message ||
                err.response?.data?.title ||
                "Unable to delete user."
            );

        } finally {
            setDeleting(false);
        }
    };


    // Table Columns
    const columns = [
        {
            key: "userId",
            label: "User ID"
        },
        {
            key: "userName",
            label: "Username"
        },
        {
            key: "email",
            label: "Email"
        },
        {
            key: "roleName",
            label: "Role"
        },
        {
            key: "isActive",
            label: "Status",
            render: (user) => (
                <StatusBadge isActive={user.isActive} />
            )
        },
        {
            key: "actions",
            label: "Actions",
            render: (user) => (
                <div className="flex items-center gap-3">

                    <button
                        onClick={() => handleEdit(user)}
                        className="px-3 py-1.5 text-sm font-medium text-blue-700 bg-blue-100 hover:bg-blue-200 rounded-lg transition"
                    >
                        Edit
                    </button>

                    <button
                        onClick={() => handleDeleteClick(user)}
                        className="px-3 py-1.5 text-sm font-medium text-red-700 bg-red-100 hover:bg-red-200 rounded-lg transition"
                    >
                        Delete
                    </button>

                </div>
            )
        }
    ];


    return (
        <div className="space-y-6">

            {/* Page Header */}
            <div className="flex items-center justify-between">

                <div>
                    <h1 className="text-2xl font-bold text-slate-800">
                        User Management
                    </h1>

                    <p className="text-slate-500 mt-1">
                        Manage application users and their accounts.
                    </p>
                </div>

                <button
                    onClick={handleAdd}
                    className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-medium transition"
                >
                    + Add User
                </button>

            </div>


            {/* Success Message */}
            {success && (
                <div className="bg-green-100 text-green-700 px-4 py-3 rounded-lg">
                    {success}
                </div>
            )}


            {/* Error Message */}
            {error && (
                <div className="bg-red-100 text-red-700 px-4 py-3 rounded-lg">
                    {error}
                </div>
            )}


            {/* Search and Filters */}
            <div className="bg-white p-5 rounded-xl border border-slate-200">

                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">

                    {/* Search */}
                    <SearchInput
                        label="Search Users"
                        value={search}
                        onChange={(e) => {
                            setSearch(e.target.value);
                            setPageNumber(1);
                        }}
                        placeholder="Search username or email..."
                    />


                    {/* Role Filter */}
                    <SelectFilter
                        label="Filter by Role"
                        value={roleFilter}
                        onChange={(e) => {
                            setRoleFilter(e.target.value);
                            setPageNumber(1);
                        }}
                        placeholder="All Roles"
                        options={roles.map(role => ({
                            value: role.id,
                            label: role.roleName
                        }))}
                    />


                    {/* Status Filter */}
                    <SelectFilter
                        label="Filter by Status"
                        value={statusFilter}
                        onChange={(e) => {
                            setStatusFilter(e.target.value);
                            setPageNumber(1);
                        }}
                        placeholder="All Statuses"
                        options={[
                            { value: "true", label: "Active" },
                            { value: "false", label: "Inactive" }
                        ]}
                    />


                    {/* Sorting */}
                    <SortSelect
                        label="Sort By"
                        value={`${sortBy}|${sortDirection}`}
                        onChange={(e) => {
                            const [column, direction] =
                                e.target.value.split("|");

                            setSortBy(column);
                            setSortDirection(direction);
                            setPageNumber(1);
                        }}
                        options={[
                            { value: "userName|asc", label: "Username (A-Z)" },
                            { value: "userName|desc", label: "Username (Z-A)" },
                            { value: "email|asc", label: "Email (A-Z)" },
                            { value: "email|desc", label: "Email (Z-A)" },
                            { value: "roleName|asc", label: "Role (A-Z)" },
                            { value: "roleName|desc", label: "Role (Z-A)" },
                            { value: "createdAt|desc", label: "Newest First" },
                            { value: "createdAt|asc", label: "Oldest First" },
                            { value: "isActive|asc", label: "Status (Ascending)" },
                            { value: "isActive|desc", label: "Status (Descending)" }
                        ]}
                    />

                </div>

            </div>


            {/* Users Table */}
            <DataTable
                columns={columns}
                data={users}
                loading={loading}
                emptyMessage="No users found."
                getRowKey={(user) => user.userId}
            />


            {/* Pagination */}
            <Pagination
                pageNumber={pageNumber}
                pageSize={pageSize}
                totalPages={totalPages}
                totalRecords={totalRecords}
                hasPreviousPage={hasPreviousPage}
                hasNextPage={hasNextPage}
                onPageChange={setPageNumber}
                onPageSizeChange={(size) => {
                    setPageSize(size);
                    setPageNumber(1);
                }}
                loading={loading}
            />


            {/* Add / Edit User Modal */}
            <Modal
                isOpen={showModal}
                onClose={handleCloseModal}
                title={selectedUser ? "Edit User" : "Add New User"}
                size="md"
            >
                <GenericForm
                    key={selectedUser?.userId ?? "new-user"}
                    fields={userFields}
                    initialData={initialFormData}
                    saving={saving}
                    submitText={selectedUser ? "Update User" : "Create User"}
                    onSubmit={handleSubmit}
                    onCancel={handleCloseModal}
                />
            </Modal>


            {/* Delete Confirmation */}
            <ConfirmDialog
                isOpen={showDeleteDialog}
                onClose={() => {
                    if (!deleting) {
                        setShowDeleteDialog(false);
                        setSelectedUser(null);
                    }
                }}
                onConfirm={handleDelete}
                title="Delete User"
                message={`Are you sure you want to delete "${selectedUser?.userName}"?`}
                confirmText="Delete"
                loading={deleting}
            />

        </div>
    );
}

export default Users;
