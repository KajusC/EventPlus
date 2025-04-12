export const formatDate = (dateString) => {
    const options = { year: 'numeric', month: 'long', day: 'numeric' };
    return new Date(dateString).toLocaleDateString(undefined, options);
};

export const formatTime = (timeString) => {
    const options = { hour: '2-digit', minute: '2-digit' };
    return new Date(`1970-01-01T${timeString}Z`).toLocaleTimeString(undefined, options);
};

export const capitalizeFirstLetter = (string) => {
    return string.charAt(0).toUpperCase() + string.slice(1);
};