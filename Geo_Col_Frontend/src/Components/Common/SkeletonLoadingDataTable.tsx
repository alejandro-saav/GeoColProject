import React from 'react';

/**
 * Renders a skeleton loading placeholder that mimics the structure
 * of the main table component, using Tailwind CSS for animation.
 *
 * This component takes no props and is intended to be shown
 * conditionally while data is fetching.
 */
export const TableSkeleton: React.FC = () => {
    // An array to easily map and create multiple skeleton rows
    const skeletonRows = [1, 2, 3, 4,5,6,7,8,9]; // 4 rows

    return (
        <div className="w-full max-w-md mx-auto p-4">
            <div className="bg-[#1a1a2e] rounded-lg shadow-2xl overflow-hidden border border-[#2a2a3e]">

                {/* --- SKELETON HEADER --- */}
                <div className="bg-gradient-to-r from-[#000030] to-[#1a1a3e] px-4 py-3 border-b border-[#2a2a3e]">
                    <div className="animate-pulse space-y-2 flex flex-col gap-2">
                        {/* Mimics the H2 header */}
                        <div className="h-6 bg-[#2a2a3e] rounded w-3/5"></div>
                        {/* Mimics the P sub-header */}
                        <div className="h-3 bg-[#2a2a3e] rounded w-2/5"></div>
                    </div>
                </div>

                {/* --- SKELETON TABLE --- */}
                <div className="overflow-x-auto">
                    <table className="w-full table-auto">
                        <thead>
                        <tr className="bg-[#0f0f1e] border-b border-[#2a2a3e]">

                            {/* 3 Columns as requested */}
                            <th className="px-3 py-2">
                                <div className="animate-pulse h-4 bg-[#2a2a3e] rounded w-3/4"></div>
                            </th>
                            <th className="px-3 py-2">
                                <div className="animate-pulse h-4 bg-[#2a2a3e] rounded w-3/4"></div>
                            </th>

                        </tr>
                        </thead>
                        <tbody className="divide-y divide-[#2a2a3e]">

                        {skeletonRows.map((row) => (
                            <tr key={row} className="bg-[#1a1a2e]">

                                {/* 3 Columns */}
                                <td className="px-3 py-4">
                                    <div className="animate-pulse h-5 bg-[#2a2a3e] rounded w-full"></div>
                                </td>
                                <td className="px-3 py-4">
                                    <div className="animate-pulse h-5 bg-[#2a2a3e] rounded w-full"></div>
                                </td>

                            </tr>
                        ))}

                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
};

export default TableSkeleton;