import { getTotalOffDays } from './common.functions';

export const createScheduleActions = () => ({
    validateScheduleChanges: (schedule, totalOffDaysDefault) => {
        const totalOffDays = getTotalOffDays(schedule);
        return totalOffDaysDefault >= totalOffDays;
    }
});

export default createScheduleActions;
