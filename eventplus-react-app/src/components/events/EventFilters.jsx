import React from 'react';

const EventFilters = () => {
    return (
        <div>
            <h2>Filter Events</h2>
            <form>
                <label>
                    Category:
                    <select>
                        <option value="">All</option>
                        <option value="Music">Music</option>
                        <option value="Art">Art</option>
                        <option value="Technology">Technology</option>
                        <option value="Theater">Theater</option>
                    </select>
                </label>
                <label>
                    Date:
                    <input type="date" />
                </label>
                <button type="submit">Apply Filters</button>
            </form>
        </div>
    );
};

export default EventFilters;