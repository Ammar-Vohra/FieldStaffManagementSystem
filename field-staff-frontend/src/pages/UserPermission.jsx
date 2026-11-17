
import { useEffect, useState } from "react";

import userService from "../services/userService";
import permissionService from "../services/permissionService";

import SelectFilter from "../components/filters/SelectFilter";

function UserPermissions() {

    // Data
    const [users, setUsers] = useState([]);
    const [permissions, setPermissions] = useState([]);

    // Selection
    const [selectedUserId, setSelectedUserId] = useState("");
    const [selectedPermissions, setSelectedPermissions] = useState(new Set());

    // Loading states
    const [loading, setLoading] = useState(true);
    const [loadingUserPermissions, setLoadingUserPermissions] = useState(false);
    const [saving, setSaving] = useState(false);

    // Messages
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    // Fetch users and available permissions
    useEffect(() => {

        const fetchInitialData = async () => {

            try {
                setLoading(true);
                setError("");

                const [usersData, permissionsData] = await Promise.all([
                    userService.getAllUsers(),
                    permissionService.getAllPermissions()
                ]);

                setUsers(
                    Array.isArray(usersData)
                        ? usersData
                        : usersData.data || []
                );

                setPermissions(
                    Array.isArray(permissionsData)
                        ? permissionsData
                        : permissionsData.data || []
                );

            } catch (err) {

                console.error("Failed to load permission data:", err);

                setError(
                    err.response?.data?.message ||
                    err.response?.data?.title ||
                    "Unable to load users and permissions."
                );

            } finally {
                setLoading(false);
            }
        };

        fetchInitialData();

    }, []);


    // Fetch permissions whenever selected user changes
    useEffect(() => {

        if (!selectedUserId) {
            setSelectedPermissions(new Set());
            return;
        }

        let isMounted = true;

        const fetchUserPermissions = async () => {

            try {
                setLoadingUserPermissions(true);
                setError("");
                setSuccess("");

                const assignedPermissions =
                    await permissionService.getUserPermissions(
                        Number(selectedUserId)
                    );

                if (!isMounted) return;

                const grantedIds = assignedPermissions
                    .filter(permission =>
                        permission.isGranted && permission.isActive
                    )
                    .map(permission => permission.permissionId);

                setSelectedPermissions(new Set(grantedIds));

            } catch (err) {

                if (!isMounted) return;

                console.error("Failed to fetch user permissions:", err);

                setError(
                    err.response?.data?.message ||
                    err.response?.data?.title ||
                    "Unable to load assigned permissions."
                );

                setSelectedPermissions(new Set());

            } finally {

                if (isMounted) {
                    setLoadingUserPermissions(false);
                }
            }
        };

        fetchUserPermissions();

        return () => {
            isMounted = false;
        };

    }, [selectedUserId]);


    // Handle checkbox selection
    const handlePermissionChange = (permissionId) => {

        setSelectedPermissions(previous => {

            const updated = new Set(previous);

            if (updated.has(permissionId)) {
                updated.delete(permissionId);
            } else {
                updated.add(permissionId);
            }

            return updated;
        });

        setSuccess("");
        setError("");
    };


    // Save permissions using bulk API
    const handleSave = async () => {

        if (!selectedUserId) {
            setError("Please select a user first.");
            return;
        }

        try {

            setSaving(true);
            setError("");
            setSuccess("");

            await permissionService.updateUserPermissions(
                Number(selectedUserId),
                Array.from(selectedPermissions)
            );

            setSuccess("User permissions updated successfully.");

        } catch (err) {

            console.error("Failed to update permissions:", err);

            setError(
                err.response?.data?.message ||
                err.response?.data?.title ||
                "Unable to update user permissions."
            );

        } finally {
            setSaving(false);
        }
    };


    const selectedUser = users.find(
        user => String(user.userId) === selectedUserId
    );


    if (loading) {
        return (
            <div className="p-6 text-slate-500">
                Loading users and permissions...
            </div>
        );
    }


    return (

        <div className="space-y-6">

            {/* Page Header */}
            <div>
                <h1 className="text-2xl font-bold text-slate-800">
                    User Permission Management
                </h1>

                <p className="text-slate-500 mt-1">
                    Manage individual permissions assigned to application users.
                </p>
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


            {/* User Selection */}
            <div className="bg-white p-5 rounded-xl border border-slate-200 space-y-4">

                <h2 className="text-lg font-semibold text-slate-800">
                    Select User
                </h2>

                <SelectFilter
                    label="Choose User"
                    value={selectedUserId}
                    onChange={(e) => {
                        setSelectedUserId(e.target.value);
                        setError("");
                        setSuccess("");
                    }}
                    placeholder="Select a user"
                    options={users.map(user => ({
                        value: String(user.userId),
                        label: `${user.userName} (${user.email})`
                    }))}
                />

                {selectedUser && (
                    <div className="text-sm text-slate-600">
                        <span className="font-medium">Selected User:</span>{" "}
                        {selectedUser.userName}
                    </div>
                )}

            </div>


            {/* Permissions */}
            {selectedUserId && (

                <div className="bg-white rounded-xl border border-slate-200">

                    <div className="p-5 border-b border-slate-200 flex items-center justify-between">

                        <div>
                            <h2 className="text-lg font-semibold text-slate-800">
                                Available Permissions
                            </h2>

                            <p className="text-sm text-slate-500 mt-1">
                                Select the permissions you want to grant.
                            </p>
                        </div>

                        <span className="text-sm font-medium text-blue-600">
                            {selectedPermissions.size} Selected
                        </span>

                    </div>


                    {loadingUserPermissions ? (

                        <div className="p-6 text-center text-slate-500">
                            Loading assigned permissions...
                        </div>

                    ) : permissions.length === 0 ? (

                        <div className="p-6 text-center text-slate-500">
                            No permissions available.
                        </div>

                    ) : (

                        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 p-5">

                            {permissions.map(permission => (

                                <label
                                    key={permission.permissionId}
                                    className="flex items-center gap-4 p-5 hover:bg-slate-50 cursor-pointer transition"
                                >

                                    <input
                                        type="checkbox"
                                        checked={selectedPermissions.has(
                                            permission.permissionId
                                        )}
                                        onChange={() =>
                                            handlePermissionChange(
                                                permission.permissionId
                                            )
                                        }
                                        className="w-4 h-4 accent-blue-600"
                                    />

                                    <div className="flex-1">

                                        <p className="font-medium text-slate-800">
                                            {permission.permissionName}
                                        </p>

                                        <p className="text-sm text-slate-500">
                                            {permission.permissionCode}
                                        </p>

                                    </div>

                                    {selectedPermissions.has(
                                        permission.permissionId
                                    ) && (

                                        <span className="text-xs font-medium text-green-600">
                                            Granted
                                        </span>

                                    )}

                                </label>

                            ))}

                        </div>

                    )}


                    {/* Save Button */}
                    <div className="p-5 border-t border-slate-200 flex justify-end">

                        <button
                            onClick={handleSave}
                            disabled={
                                saving ||
                                loadingUserPermissions ||
                                !selectedUserId
                            }
                            className="px-5 py-2.5 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-medium transition disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                            {saving ? "Saving..." : "Save Permissions"}
                        </button>

                    </div>

                </div>

            )}

        </div>
    );
}

export default UserPermissions;
