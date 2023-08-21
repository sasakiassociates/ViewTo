import { stores } from '@strategies/stores';
import { action, computed, makeObservable, observable } from 'mobx';

import Stores from '../stores/Stores';


export default class ReferenceObject {

    readonly id: string;

    constructor(id: string) {
        makeObservable(this);
        this.id = id;
        this.load();
    }

    @observable
    isLoading: boolean = false;

    @action
    setIsLoading(isLoading = true) {
        this.isLoading = isLoading;
    }

    @computed
    get hasLoaded() {
        return this.data !== undefined;
    }

    @observable
    data: any = undefined;

    @action
    setData(data: any) {
        this.data = data;
    }

    @observable
    referenceObject: string = "";

    @action
    setReference(data: string) {
        this.referenceObject = data;
    }

    @action
    async load() {
        if (!this.isLoading) {
            const { speckle, project } = (stores as Stores).scenario;
            this.setIsLoading();
            const obj = await speckle.Project(project.id).Version(this.id).get;
            this.setData(obj);
            this.setReference(obj.referencedObject);
            this.setIsLoading(false);
        }
    }

}
