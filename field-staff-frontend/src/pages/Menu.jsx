
import { useCallback, useEffect, useMemo, useState } from "react";

import api from "../services/api";
import menuService from "../services/menuService";
import roleMenuService from "../services/roleMenuService";
import roleService from "../services/roleService";

import Modal from "../components/common/Modal";
import GenericForm from "../components/common/GenericForm";
import ConfirmDialogue from "../components/common/ConfirmDialogue";
import DataTable from "../components/common/DataTable";
import StatusBadge from "../components/common/StatusBadge";

import Pagination from "../components/data-display/Pagination";
import SearchInput from "../components/filters/SearchInput";
import SelectFilter from "../components/filters/SelectFilter";
import SortSelect from "../components/filters/SortSelect";


function Menu() {

    // =========================
    // MENU MANAGEMENT STATE
    // =========================

    const [menus, setMenus] = useState([]);
    const [allMenus, setAllMenus] = useState([]);

    const [loading, setLoading] = useState(false);
    const [saving, setSaving] = useState(false);

    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isDeleteOpen, setIsDeleteOpen] = useState(false);

    const [selectedMenu, setSelectedMenu] = useState(null);
    const [menuToDelete, setMenuToDelete] = useState(null);

    // Pagination
    const [pageNumber, setPageNumber] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(0);
    const [totalRecords, setTotalRecords] = useState(0);

    // Filters
    const [search, setSearch] = useState("");
    const [isActive, setIsActive] = useState("");
    const [parentMenuId, setParentMenuId] = useState("");

    // Sorting
    const [sortBy, setSortBy] = useState("menuName");
    const [sortDirection, setSortDirection] = useState("asc");


    // =========================
    // ROLE MENU STATE
    // =========================

    const [roles, setRoles] = useState([]);
    const [myMenus, setMyMenus] = useState([]);
    const [roleMenus, setRoleMenus] = useState([]);

    const [selectedRoleId, setSelectedRoleId] = useState("");

    const [selectedMenuIds, setSelectedMenuIds] = useState([]);

    const [roleMenuLoading, setRoleMenuLoading] = useState(false);
    const [roleMenuSaving, setRoleMenuSaving] = useState(false);

    const [roleMenuToDelete, setRoleMenuToDelete] = useState(null);
    const [isRoleMenuDeleteOpen, setIsRoleMenuDeleteOpen] = useState(false);


    // =========================
    // HELPER: EXTRACT ARRAY
    // =========================

    const extractArray = (response) => {

        if (Array.isArray(response)) {
            return response;
        }

        if (Array.isArray(response?.data)) {
            return response.data;
        }

        if (Array.isArray(response?.items)) {
            return response.items;
        }

        return [];

    };


    // =========================
    // FETCH ALL MENUS
    // =========================

    const fetchAllMenus = useCallback(async () => {

        try {

            const response = await menuService.getAllMenus();

            const data = extractArray(response);

            setAllMenus(data);

        } catch (err) {

            console.error("Failed to fetch all menus:", err);

        }

    }, []);


    // =========================
    // FETCH PAGINATED MENUS
    // =========================

    const fetchMenus = useCallback(async () => {

        try {

            setLoading(true);
            setError("");

            const response = await menuService.getPaginatedMenus({

                pageNumber,
                pageSize,
                search,
                sortBy,
                sortDirection,

                isActive: isActive === ""
                    ? undefined
                    : isActive === "true",

                parentMenuId: parentMenuId === ""
                    ? undefined
                    : Number(parentMenuId)

            });

            setMenus(response.data ?? []);
            setTotalPages(response.totalPages ?? 0);
            setTotalRecords(response.totalRecords ?? 0);

        } catch (err) {

            console.error("Failed to fetch menus:", err);

            setError(
                err.response?.data?.message ||
                err.response?.data?.title ||
                "Failed to load menus."
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
        isActive,
        parentMenuId
    ]);


    // =========================
    // FETCH ROLE MENU DATA
    // =========================

    
const fetchRoleMenuData = useCallback(async () => {

    try {

        setRoleMenuLoading(true);
        setError("");

        const [
            myMenusResponse,
            roleMenusResponse,
            rolesResponse
        ] = await Promise.all([

            roleMenuService.getMyMenu(),
            roleMenuService.getAllMenus(),
            roleService.getAllRoles()

        ]);

        const myMenusData = extractArray(myMenusResponse);

        const roleMenusData = extractArray(roleMenusResponse);

        // FIX: rolesResponse is already response.data
        const rolesData = extractArray(rolesResponse)
            .filter(role => role.isActive === true);

        console.log("My Menus:", myMenusData);
        console.log("Role Menus:", roleMenusData);
        console.log("All Roles Response:", rolesResponse);
        console.log("Filtered Roles:", rolesData);

        setMyMenus(myMenusData);
        setRoleMenus(roleMenusData);
        setRoles(rolesData);

        // Automatically select a role
        setSelectedRoleId(previousId => {

            const stillExists = rolesData.some(
                role => String(role.id) === String(previousId)
            );

            if (stillExists) {
                return previousId;
            }

            const currentRoleId = myMenusData[0]?.roleId;

            const otherRole = rolesData.find(
                role => String(role.id) !== String(currentRoleId)
            );

            return String(
                (otherRole || rolesData[0])?.id ?? ""
            );

        });

    } catch (err) {

        console.error("Failed to fetch role menus:", err);

        setError(
            err.response?.data?.message ||
            err.response?.data?.title ||
            "Failed to load role menu information."
        );

    } finally {

        setRoleMenuLoading(false);

    }

}, []);


    // =========================
    // INITIAL FETCH
    // =========================

    useEffect(() => {

        fetchAllMenus();

        fetchRoleMenuData();

    }, [fetchAllMenus, fetchRoleMenuData]);


    useEffect(() => {

        fetchMenus();

    }, [fetchMenus]);


    // =========================
    // SELECTED ROLE ASSIGNMENTS
    // =========================

    const assignedRoleMenus = useMemo(() => {

        return roleMenus.filter(roleMenu =>

            Number(roleMenu.roleId) === Number(selectedRoleId) &&
            roleMenu.isActive === true

        );

    }, [roleMenus, selectedRoleId]);


    // =========================
    // AVAILABLE MENUS
    // =========================

    const availableMenus = useMemo(() => {

        const assignedMenuIds = assignedRoleMenus.map(
            roleMenu => Number(roleMenu.menuId)
        );

        return allMenus.filter(menu =>

            menu.isActive &&
            !assignedMenuIds.includes(Number(menu.menuId))

        );

    }, [allMenus, assignedRoleMenus]);


    // =========================
    // ROLE OPTIONS
    // =========================

    const roleOptions = useMemo(() => {

        return roles.map(role => ({

            value: String(role.id),

            label: role.roleName

        }));

    }, [roles]);


    // =========================
    // ASSIGN MENU CHECKBOX
    // =========================

    const handleMenuSelection = (menuId) => {

        const id = Number(menuId);

        setSelectedMenuIds(previous => {

            if (previous.includes(id)) {

                return previous.filter(item => item !== id);

            }

            return [...previous, id];

        });

    };


    // =========================
    // ASSIGN SELECTED MENUS
    // =========================

    const handleAssignMenus = async () => {

        if (!selectedRoleId || selectedMenuIds.length === 0) {

            setError("Please select a role and at least one menu.");

            return;

        }

        try {

            setRoleMenuSaving(true);

            setError("");
            setSuccess("");

            let assignedCount = 0;

            // Individual POST requests because there is no bulk endpoint
            for (const menuId of selectedMenuIds) {

                await roleMenuService.createRoleMenuById({

                    roleId: Number(selectedRoleId),

                    menuId: Number(menuId),

                    isActive: true

                });

                assignedCount++;

            }

            setSelectedMenuIds([]);

            setSuccess(
                `${assignedCount} menu(s) assigned successfully.`
            );

            await fetchRoleMenuData();

        } catch (err) {

            console.error("Failed to assign menus:", err);

            setError(
                err.response?.data?.message ||
                err.response?.data?.title ||
                "Failed to assign menus. Some assignments may already have been saved."
            );

            // Refresh to reflect any successful requests
            await fetchRoleMenuData();

        } finally {

            setRoleMenuSaving(false);

        }

    };


    // =========================
    // REMOVE ROLE MENU
    // =========================

    const handleRoleMenuDeleteClick = (roleMenu) => {

        setRoleMenuToDelete(roleMenu);

        setIsRoleMenuDeleteOpen(true);

    };


    const handleRoleMenuDeleteConfirm = async () => {

        if (!roleMenuToDelete) return;

        try {

            setError("");
            setSuccess("");

            await roleMenuService.deleteRoleMenuById(
                roleMenuToDelete.roleMenuId
            );

            setSuccess(
                `${roleMenuToDelete.menuName} removed from the selected role.`
            );

            setIsRoleMenuDeleteOpen(false);
            setRoleMenuToDelete(null);

            await fetchRoleMenuData();

        } catch (err) {

            console.error("Failed to remove role menu:", err);

            setError(
                err.response?.data?.message ||
                err.response?.data?.title ||
                "Failed to remove menu assignment."
            );

        }

    };


    // =========================
    // CURRENT ROLE NAME
    // =========================

    const currentRoleName = myMenus[0]?.roleName || "Current Role";


    // =========================
    // FILTER HANDLERS
    // =========================

    const handleSearch = (value) => {

        setSearch(value);
        setPageNumber(1);

    };


    const handleStatusFilter = (value) => {

        setIsActive(value);
        setPageNumber(1);

    };


    const handleParentFilter = (value) => {

        setParentMenuId(value);
        setPageNumber(1);

    };


    // =========================
    // SORT HANDLERS
    // =========================

    const handleSort = (e) => {

        setSortBy(e.target.value);
        setPageNumber(1);

    };


    const handleSortDirection = () => {

        setSortDirection(prev =>
            prev === "asc" ? "desc" : "asc"
        );

        setPageNumber(1);

    };


    // =========================
    // PARENT MENU OPTIONS
    // =========================

    const parentMenuOptions = useMemo(() => {

        return allMenus

            .filter(menu =>
                menu.parentMenuId == null &&
                !menu.menuPath &&
                menu.isActive
            )

            .map(menu => ({

                value: String(menu.menuId),

                label: menu.menuName

            }));

    }, [allMenus]);


    // =========================
    // FORM FIELDS
    // =========================

    const formFields = useMemo(() => [

        {
            name: "menuName",
            label: "Menu Name",
            type: "text",
            required: true,
            placeholder: "Enter menu name"
        },

        {
            name: "parentMenuId",
            label: "Parent Menu",
            type: "select",
            options: [
                { value: "", label: "No Parent (Main Menu)" },
                ...parentMenuOptions
            ]
        },

        {
            name: "menuPath",
            label: "Menu Path",
            type: "text",
            placeholder: "e.g. /users"
        },

        {
            name: "icon",
            label: "Icon",
            type: "text",
            placeholder: "e.g. Users"
        },

        {
            name: "sortOrder",
            label: "Sort Order",
            type: "number",
            required: true,
            placeholder: "Enter sort order"
        },

        {
            name: "isActive",
            label: "Active",
            type: "checkbox"
        }

    ], [parentMenuOptions]);


    // =========================
    // OPEN CREATE MODAL
    // =========================

    const handleAdd = () => {

        setSelectedMenu(null);
        setError("");
        setSuccess("");

        setIsModalOpen(true);

    };


    // =========================
    // OPEN EDIT MODAL
    // =========================

    const handleEdit = (menu) => {

        setSelectedMenu({

            ...menu,

            parentMenuId: menu.parentMenuId
                ? String(menu.parentMenuId)
                : "",

            sortOrder: menu.sortOrder ?? 0,

            isActive: menu.isActive ?? true

        });

        setError("");
        setSuccess("");

        setIsModalOpen(true);

    };


    // =========================
    // CREATE / UPDATE MENU
    // =========================

    const handleSubmit = async (formData) => {

        try {

            setSaving(true);
            setError("");
            setSuccess("");

            const payload = {

                menuName: formData.menuName?.trim(),

                parentMenuId: formData.parentMenuId
                    ? Number(formData.parentMenuId)
                    : null,

                menuPath: formData.menuPath?.trim() || null,

                icon: formData.icon?.trim() || null,

                sortOrder: Number(formData.sortOrder) || 0,

                isActive: Boolean(formData.isActive)

            };


            // Child menus must have a path
            if (payload.parentMenuId && !payload.menuPath) {

                setError("A child menu must have a menu path.");

                return;

            }


            if (selectedMenu) {

                await menuService.updateMenu(
                    selectedMenu.menuId,
                    payload
                );

                setSuccess("Menu updated successfully.");

            } else {

                await menuService.createMenu(payload);

                setSuccess("Menu created successfully.");

            }

            setIsModalOpen(false);
            setSelectedMenu(null);

            await fetchAllMenus();
            await fetchMenus();

        } catch (err) {

            console.error("Failed to save menu:", err);

            setError(
                err.response?.data?.message ||
                err.response?.data?.title ||
                "Failed to save menu."
            );

        } finally {

            setSaving(false);

        }

    };


    // =========================
    // DELETE MENU
    // =========================

    const handleDeleteClick = (menu) => {

        setMenuToDelete(menu);

        setIsDeleteOpen(true);

    };


    const handleDeleteConfirm = async () => {

        if (!menuToDelete) return;

        try {

            setError("");
            setSuccess("");

            await menuService.deleteMenu(menuToDelete.menuId);

            setSuccess("Menu deleted successfully.");

            setIsDeleteOpen(false);
            setMenuToDelete(null);

            await fetchAllMenus();
            await fetchMenus();
            await fetchRoleMenuData();

        } catch (err) {

            console.error("Failed to delete menu:", err);

            setError(
                err.response?.data?.message ||
                err.response?.data?.title ||
                "Failed to delete menu."
            );

        }

    };


    // =========================
    // TABLE COLUMNS
    // =========================

    const columns = [

        {
            key: "menuName",
            label: "Menu Name"
        },

        {
            key: "parentMenuName",
            label: "Parent Menu",
            render: (menu) => menu.parentMenuName || "-"
        },

        {
            key: "menuPath",
            label: "Menu Path",
            render: (menu) => menu.menuPath || "-"
        },

        // {
        //     key: "icon",
        //     label: "Icon",
        //     render: (menu) => menu.icon || "-"
        // },

        // {
        //     key: "sortOrder",
        //     label: "Sort Order"
        // },

        {
            key: "isActive",
            label: "Status",
            render: (menu) => (
                <StatusBadge isActive={menu.isActive} />
            )
        }

    ];


    // =========================
    // RENDER CURRENT ROLE MENUS
    // =========================

    const renderMyMenus = (items, level = 0) => {

        return items.map(menu => (

            <div key={menu.menuId}>

                <div
                    className="flex items-center justify-between py-3 border-b border-slate-100"
                    style={{ paddingLeft: `${level * 24}px` }}
                >

                    <div className="flex items-center gap-3">

                        <div className="w-9 h-9 rounded-lg bg-blue-50 text-blue-600 flex items-center justify-center">

                            <span className="text-xs font-semibold">
                                {menu.menuName?.charAt(0)}
                            </span>

                        </div>

                        <div>

                            <p className="text-sm font-medium text-slate-800">
                                {menu.menuName}
                            </p>

                            <p className="text-xs text-slate-400">
                                {menu.menuPath || "Parent Menu"}
                            </p>

                        </div>

                    </div>

                    <StatusBadge isActive={menu.isActive} />

                </div>

                {menu.children?.length > 0 && (

                    <div>
                        {renderMyMenus(menu.children, level + 1)}
                    </div>

                )}

            </div>

        ));

    };


    // =========================
    // RENDER
    // =========================

    return (

        <div className="space-y-6">

            {/* HEADER */}

            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">

                <div>

                    <h1 className="text-2xl font-bold text-slate-800">
                        Menu Management
                    </h1>

                    <p className="text-sm text-slate-500 mt-1">
                        Manage application menus and role-based menu assignments.
                    </p>

                </div>

                <button
                    onClick={handleAdd}
                    className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
                >
                    + Add Menu
                </button>

            </div>


            {/* MESSAGES */}

            {error && (

                <div className="p-3 rounded-lg bg-red-50 text-red-700 text-sm">
                    {error}
                </div>

            )}

            {success && (

                <div className="p-3 rounded-lg bg-green-50 text-green-700 text-sm">
                    {success}
                </div>

            )}


            {/* ================================= */}
            {/* SECTION 1: MY ROLE MENUS */}
            {/* ================================= */}

            <div className="bg-white rounded-xl border border-slate-200 shadow-sm p-5">

                <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 mb-5">

                    <div>

                        <h2 className="text-lg font-semibold text-slate-800">
                            My Role Menus
                        </h2>

                        <p className="text-sm text-slate-500 mt-1">
                            Menus currently accessible through your assigned role.
                        </p>

                    </div>

                    <div className="flex items-center gap-2">

                        <span className="px-3 py-1.5 text-sm font-medium bg-blue-50 text-blue-700 rounded-lg">
                            {currentRoleName}
                        </span>

                        <span className="px-3 py-1.5 text-sm font-medium bg-slate-100 text-slate-700 rounded-lg">
                            {myMenus.length} Menus
                        </span>

                    </div>

                </div>


                {roleMenuLoading ? (

                    <div className="text-center py-8 text-slate-500 text-sm">
                        Loading your role menus...
                    </div>

                ) : myMenus.length === 0 ? (

                    <div className="text-center py-8 text-slate-500 text-sm">
                        No menus are currently assigned to your role.
                    </div>

                ) : (

                    <div className="border border-slate-100 rounded-lg px-4">

                        {renderMyMenus(myMenus)}

                    </div>

                )}

            </div>


            {/* ================================= */}
            {/* SECTION 2: MANAGE ROLE MENUS */}
            {/* ================================= */}

            <div className="bg-white rounded-xl border border-slate-200 shadow-sm p-5">

                <div className="mb-5">

                    <h2 className="text-lg font-semibold text-slate-800">
                        Manage Role Menus
                    </h2>

                    <p className="text-sm text-slate-500 mt-1">
                        Assign additional menus to roles or remove existing assignments.
                    </p>

                </div>


                {/* SELECT ROLE */}

                <div className="max-w-md mb-6">

                    <SelectFilter
                        label="Select Role"
                        value={selectedRoleId}
                        onChange={(e) => {

                            setSelectedRoleId(e.target.value);
                            setSelectedMenuIds([]);

                            setError("");
                            setSuccess("");

                        }}
                        options={[
                            {
                                value: "",
                                label: "Select a role"
                            },
                            ...roleOptions
                        ]}
                    />

                </div>


                {selectedRoleId && (

                    <>

                        {/* ASSIGNED MENUS */}

                        <div className="mb-7">

                            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-2 mb-3">

                                <h3 className="text-sm font-semibold text-slate-800">
                                    Currently Assigned Menus
                                </h3>

                                <span className="text-xs text-slate-500">
                                    {assignedRoleMenus.length} assigned
                                </span>

                            </div>


                            {roleMenuLoading ? (

                                <div className="text-center py-6 text-sm text-slate-500">
                                    Loading assignments...
                                </div>

                            ) : assignedRoleMenus.length === 0 ? (

                                <div className="p-5 text-center border border-dashed border-slate-300 rounded-lg text-sm text-slate-500">
                                    No menus assigned to this role.
                                </div>

                            ) : (

                                <div className="border border-slate-200 rounded-lg divide-y divide-slate-100">

                                    {assignedRoleMenus.map(roleMenu => (

                                        <div
                                            key={roleMenu.roleMenuId}
                                            className="flex items-center justify-between gap-3 px-4 py-3"
                                        >

                                            <div className="min-w-0">

                                                <p className="text-sm font-medium text-slate-800">
                                                    {roleMenu.menuName}
                                                </p>

                                                <p className="text-xs text-slate-400 mt-0.5">
                                                    {roleMenu.menuPath || "Parent Menu"}
                                                </p>

                                            </div>


                                            <button
                                                type="button"
                                                onClick={() => handleRoleMenuDeleteClick(roleMenu)}
                                                className="shrink-0 px-3 py-1.5 text-xs font-medium text-red-600 border border-red-200 rounded-lg hover:bg-red-50 transition"
                                            >
                                                Remove
                                            </button>

                                        </div>

                                    ))}

                                </div>

                            )}

                        </div>


                        {/* ADDITIONAL MENUS */}

                        <div className="border-t border-slate-200 pt-6">

                            <div className="mb-4">

                                <h3 className="text-sm font-semibold text-slate-800">
                                    Add Additional Menus
                                </h3>

                                <p className="text-xs text-slate-500 mt-1">
                                    Select menus that are not currently assigned to this role.
                                </p>

                            </div>


                            {roleMenuLoading ? (

                                <div className="text-center py-6 text-sm text-slate-500">
                                    Loading available menus...
                                </div>

                            ) : availableMenus.length === 0 ? (

                                <div className="p-5 text-center border border-dashed border-slate-300 rounded-lg text-sm text-slate-500">
                                    All active menus are already assigned to this role.
                                </div>

                            ) : (

                                <div className="border border-slate-200 rounded-lg p-4">

                                    <div className="flex items-center justify-between mb-3">

                                        <p className="text-sm font-medium text-slate-700">
                                            Available Menus
                                        </p>

                                        <span className="text-xs text-slate-500">
                                            {availableMenus.length} available
                                        </span>

                                    </div>


                                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">

                                        {availableMenus.map(menu => (

                                            <label
                                                key={menu.menuId}
                                                className={`flex items-center gap-3 p-3 border rounded-lg cursor-pointer transition ${
                                                    selectedMenuIds.includes(Number(menu.menuId))
                                                        ? "border-blue-400 bg-blue-50"
                                                        : "border-slate-200 hover:bg-slate-50"
                                                }`}
                                            >

                                                <input
                                                    type="checkbox"
                                                    checked={selectedMenuIds.includes(Number(menu.menuId))}
                                                    onChange={() => handleMenuSelection(menu.menuId)}
                                                    className="w-4 h-4 accent-blue-600"
                                                />

                                                <span className="text-sm text-slate-700">
                                                    {menu.menuName}
                                                </span>

                                            </label>

                                        ))}

                                    </div>


                                    <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 mt-5 pt-4 border-t border-slate-100">

                                        <p className="text-sm text-slate-500">
                                            {selectedMenuIds.length} menu(s) selected
                                        </p>


                                        <button
                                            type="button"
                                            onClick={handleAssignMenus}
                                            disabled={
                                                selectedMenuIds.length === 0 ||
                                                roleMenuSaving
                                            }
                                            className="px-5 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 disabled:bg-slate-300 disabled:cursor-not-allowed transition"
                                        >

                                            {roleMenuSaving
                                                ? "Assigning..."
                                                : "Assign Selected Menus"
                                            }

                                        </button>

                                    </div>

                                </div>

                            )}

                        </div>

                    </>

                )}

            </div>


            {/* ================================= */}
            {/* SECTION 3: EXISTING MENU FILTERS */}
            {/* ================================= */}

            <div className="bg-white p-4 rounded-xl border border-slate-200 shadow-sm">

                <div className="mb-4">

                    <h2 className="text-lg font-semibold text-slate-800">
                        All Application Menus
                    </h2>

                    <p className="text-sm text-slate-500 mt-1">
                        Create, update and manage application menus.
                    </p>

                </div>


                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">

                    <SearchInput
                        value={search}
                        onChange={(e) => handleSearch(e.target.value)}
                        placeholder="Search menus..."
                    />


                    <SelectFilter
                        label="Status"
                        value={isActive}
                        onChange={(e) => handleStatusFilter(e.target.value)}
                        options={[
                            { value: "", label: "All Status" },
                            { value: "true", label: "Active" },
                            { value: "false", label: "Inactive" }
                        ]}
                    />


                    <SelectFilter
                        label="Parent Menu"
                        value={parentMenuId}
                        onChange={(e) => handleParentFilter(e.target.value)}
                        options={[
                            { value: "", label: "All Parent Menus" },
                            ...parentMenuOptions
                        ]}
                    />


                    <SortSelect
                        value={sortBy}
                        onChange={handleSort}
                        options={[
                            { value: "menuName", label: "Menu Name" },
                            { value: "menuPath", label: "Menu Path" },
                            { value: "parentMenuName", label: "Parent Menu" },
                            { value: "sortOrder", label: "Sort Order" },
                            { value: "isActive", label: "Status" },
                            { value: "createdAt", label: "Created Date" }
                        ]}
                    />

                </div>


                <div className="flex justify-end mt-4">

                    <button
                        onClick={handleSortDirection}
                        className="px-3 py-2 text-sm border border-slate-200 rounded-lg hover:bg-slate-50 text-slate-600 transition"
                    >
                        {sortDirection === "asc"
                            ? "↑ Ascending"
                            : "↓ Descending"}
                    </button>

                </div>

            </div>


            {/* ================================= */}
            {/* EXISTING MENU TABLE */}
            {/* ================================= */}

            <div className="bg-white rounded-xl border border-slate-200 shadow-sm">

                <DataTable
                    columns={columns}
                    data={menus}
                    loading={loading}
                    onEdit={handleEdit}
                    onDelete={handleDeleteClick}
                />

            </div>


            {/* PAGINATION */}

            <Pagination
                pageNumber={pageNumber}
                pageSize={pageSize}
                totalPages={totalPages}
                totalRecords={totalRecords}
                onPageChange={setPageNumber}
                onPageSizeChange={(size) => {

                    setPageSize(Number(size));
                    setPageNumber(1);

                }}

                loading={loading}
            />


            {/* ================================= */}
            {/* CREATE / EDIT MODAL */}
            {/* ================================= */}

            <Modal
                isOpen={isModalOpen}
                onClose={() => {

                    setIsModalOpen(false);
                    setSelectedMenu(null);

                }}
                title={selectedMenu ? "Edit Menu" : "Add Menu"}
            >

                <GenericForm
                    fields={formFields}
                    initialData={selectedMenu || {

                        menuName: "",
                        parentMenuId: "",
                        menuPath: "",
                        icon: "",
                        sortOrder: 0,
                        isActive: true

                    }}
                    onSubmit={handleSubmit}
                    onCancel={() => {

                        setIsModalOpen(false);
                        setSelectedMenu(null);

                    }}
                    submitLabel={selectedMenu ? "Update Menu" : "Create Menu"}
                    loading={saving}
                />

            </Modal>


            {/* ================================= */}
            {/* DELETE MENU CONFIRMATION */}
            {/* ================================= */}

            <ConfirmDialogue
                isOpen={isDeleteOpen}
                onClose={() => {

                    setIsDeleteOpen(false);
                    setMenuToDelete(null);

                }}
                onConfirm={handleDeleteConfirm}
                title="Delete Menu"
                message={`Are you sure you want to delete "${menuToDelete?.menuName}"?`}
                confirmText="Delete"
                cancelText="Cancel"
            />


            {/* ================================= */}
            {/* REMOVE ROLE MENU CONFIRMATION */}
            {/* ================================= */}

            <ConfirmDialogue
                isOpen={isRoleMenuDeleteOpen}
                onClose={() => {

                    setIsRoleMenuDeleteOpen(false);
                    setRoleMenuToDelete(null);

                }}
                onConfirm={handleRoleMenuDeleteConfirm}
                title="Remove Menu Assignment"
                message={`Are you sure you want to remove "${roleMenuToDelete?.menuName}" from this role?`}
                confirmText="Remove"
                cancelText="Cancel"
            />

        </div>

    );

}

export default Menu;

